using MelonLoader;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

namespace BLChaos.Effects;

internal class MyMemeFolder : EffectBase
{
    public MyMemeFolder() : base("My Meme Folder", 30) { }
    private static readonly string[] vidPaths = GlobalVariables.ResourcePaths.Where(p => p.Contains("video") || p.Contains("memefolder")).ToArray();
    static GameObject vidParent;


    public override void HandleNetworkMessage(byte[][] data)
    {
        // get the data we need out of the byte array array (lol)
        string vidPath = Encoding.ASCII.GetString(data[1]);

        #region Anti fuckup checks

        if (!vidPaths.Contains(vidPath))
        {
            Chaos.Warn("Meme path not found! -> " + vidPath);
            return;
        }

        VideoClip vid = GlobalVariables.EffectResources.LoadAsset(vidPath).Cast<VideoClip>();
#if DEBUG
        if (vid == null)
        {
            Chaos.Warn("Video is null, but the path exists in the resources... What?");
        }
#endif
        #endregion

        GameObject sign = Utilities.SpawnAd("");
        // flip it upside down because the render texture is upside down
        sign.transform.DeserializePosRot(data[0]);

        MeshRenderer rend = sign.GetComponentInChildren<MeshRenderer>();
        rend.material = CreateRTex(vid, out VideoPlayer plyr);

        AudioSource asource = sign.AddComponent<AudioSource>();
        asource.volume = 0.1f;
        asource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
        plyr.SetTargetAudioSource(0, asource);
        plyr.Play();
        MelonCoroutines.Start(StopVideo(plyr));
    }

    public override void OnEffectStart()
    {
        if (vidParent == null) vidParent = new GameObject("My Meme Folder Video Parent");

        string vidPath = vidPaths.Random();
        GameObject sign = Utilities.SpawnAd("");
        // flip it upside down because the render texture is upside down
        sign.transform.rotation = Quaternion.Euler(sign.transform.rotation.eulerAngles + new Vector3(0, 0, 180));
        SendNetworkData(sign.transform.SerializePosRot(), Encoding.ASCII.GetBytes(vidPath));

        VideoClip vid = GlobalVariables.EffectResources.LoadAsset(vidPath).Cast<VideoClip>();

        MeshRenderer rend = sign.GetComponentInChildren<MeshRenderer>();
        rend.material = CreateRTex(vid, out VideoPlayer plyr);

        AudioSource asource = sign.AddComponent<AudioSource>();
        asource.volume = 0.1f;
        asource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
        plyr.SetTargetAudioSource(0, asource);
        plyr.Play();
        MelonCoroutines.Start(StopVideo(plyr));
    }

    private IEnumerator StopVideo(VideoPlayer player)
    {
        if (player.clip.length > 5)
        {
            player.isLooping = true;
            yield return new WaitForSeconds(5);
            player.isLooping = false;
            yield break;
        }

        yield return new WaitForSeconds(Duration);
        player.Stop();
    }

    private Material CreateRTex(VideoClip clip, out VideoPlayer plyr)
    {
        Material mat = new Material(Shader.Find(Const.URP_LIT_NAME));
        RenderTexture tex = new RenderTexture(256, 256, 24);
        mat.SetTexture(Const.URP_MAINTEX_NAME, tex);

        VideoPlayer player = vidParent.AddComponent<VideoPlayer>();
        player.clip = clip;
        player.targetTexture = tex;
        plyr = player;

        return mat;
    }
}
