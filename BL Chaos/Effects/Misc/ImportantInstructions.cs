using MelonLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace BLChaos.Effects;

internal class ImportantInstructions : EffectBase
{
    // Can't put Init() in constructor like the other texture effects. Why? I didn't care enough to fix, so don't know!
    public ImportantInstructions() : base("Important Instructions", 15) { }

    GameObject playerParent;
    VideoClip[] videos;
    VideoPlayer videoPlayer;
    RenderTexture texture;
    Material vidMat;

    private void Init()
    {
        if (playerParent == null) playerParent = new GameObject("BWChaos Instruction Parent") { hideFlags = HideFlags.DontUnloadUnusedAsset };
        // Load the videos using a lambda in linq cause otherwise id have to for(){} it
        if (videos == null || videos[0] == null)
            videos = GlobalVariables.ResourcePaths
                                    .Where(a => a.Contains("/instructionals/"))
                                    .Select(a => GlobalVariables.EffectResources.LoadAsset<VideoClip>(a))
                                    .ToArray();
        // In case IL2 feels like fucking my shit up.
        videos.ForEach(v => v.hideFlags = HideFlags.DontUnloadUnusedAsset);


        // Create the video player and render texture
        videoPlayer = playerParent.AddComponent<VideoPlayer>();
        videoPlayer.hideFlags = HideFlags.DontUnloadUnusedAsset;
        texture = new RenderTexture(512, 512, 24);
        texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        videoPlayer.targetTexture = texture;


        // Can't make a blank material without a shader or basing it off an already existing material, so grab vr standard
        vidMat = new Material(Shader.Find("Valve/vr_standard"));
        vidMat.hideFlags = HideFlags.DontUnloadUnusedAsset;
        vidMat.mainTexture = texture;
#if DEBUG
        Log("Loaded " + videos.Length + " textures into ImportantInstructions and made render texture");
        videos.ForEach(v => Log("Instructional " + v.name + " is loaded"));
#endif
    }

    public override void OnEffectStart()
    {
        #region Initialize
        if (videos == null || videos[0] == null || videoPlayer == null) Init();
        #endregion

        if (isNetworked) return;

        GameObject sign = Utilities.SpawnAd("");
        sign.transform.rotation = Quaternion.Euler(0, 0, 180);
        // Get the "Mesh" child, and replace its meshrenderer material
        MeshRenderer rend = sign.transform.GetComponentInChildren<MeshRenderer>();
        rend.material = vidMat;
        videoPlayer.clip = videos.Random();
        SendNetworkData(videoPlayer.clip.name);

#if DEBUG
        Log($"Set material - Video: {videoPlayer.clip.name}, {videoPlayer.clip.length}s ({videoPlayer.length}s)");
#endif

        // Create/get audiosource
        AudioSource source = sign.AddComponent<AudioSource>();
        source.volume = 0f;
        source.rolloffFactor = 0.75f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.outputAudioMixerGroup = GlobalVariables.MusicMixer;

        // Make the audiosource audible and play the video's sound
        videoPlayer.SetTargetAudioSource(0, source);
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
        videoPlayer.Play();

        MelonCoroutines.Start(ModulateVolume());
        if (videoPlayer.clip.name == "you should kys... NOW") MelonCoroutines.Start(LTG()); // todo: dont hardcode this (but wtf else do i do lol)
    }

    public override void HandleNetworkMessage(string data)
    {
        GameObject sign = Utilities.SpawnAd("");
        sign.transform.rotation = Quaternion.Euler(0, 0, 180);
        // Get the "Mesh" child, and replace its meshrenderer material
        MeshRenderer rend = sign.transform.GetComponentInChildren<MeshRenderer>();
        rend.material = vidMat;
        videoPlayer.clip = videos.FirstOrDefault(v => v.name == data);
        if (videoPlayer.clip == null)
        {
            Chaos.Warn("Video not found in client -> " + data);
            return;
        }

#if DEBUG
        Log($"Set material - Video: {videoPlayer.clip.name}, {videoPlayer.clip.length}s ({videoPlayer.length}s)");
#endif

        // Create/get audiosource
        AudioSource source = sign.GetComponent<AudioSource>() ?? sign.AddComponent<AudioSource>();
        source.volume = 0f;
        source.rolloffFactor = 0.75f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.outputAudioMixerGroup = GlobalVariables.MusicMixer;

        // Make the audiosource audible and play the video's sound
        videoPlayer.SetTargetAudioSource(0, source);
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
        videoPlayer.Play();

        MelonCoroutines.Start(ModulateVolume());
        if (videoPlayer.clip.name == "you should kys... NOW") MelonCoroutines.Start(LTG()); // todo: dont hardcode this (but wtf else do i do lol)
    }

    private System.Collections.IEnumerator ModulateVolume()
    {

        float time = 0;
        yield return null;
        while (Active && time < 1.1f)
        {
            videoPlayer.SetDirectAudioVolume(0, (0f, 0.3f).Interpolate(time += Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        while (Active && videoPlayer.time < videoPlayer.length)
        {
            videoPlayer.SetDirectAudioVolume(0, (0.0f, 0.3f).Interpolate((float)(videoPlayer.length - videoPlayer.time)));
            yield return new WaitForFixedUpdate();
        }
        ForceEnd();
    }

    private System.Collections.IEnumerator LTG()
    {
#if DEBUG
        Log("Running LTG");
#endif
        yield return new WaitForSeconds(5.833f);
        if (!Active) yield break;
        GlobalVariables.Player_Health?.Death();
    }
}
