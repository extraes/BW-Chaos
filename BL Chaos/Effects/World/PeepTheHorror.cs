using BoneLib.RandomShit;
using MelonLoader;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BLChaos.Effects;

internal class PeepTheHorror : EffectBase
{
    public PeepTheHorror() : base("Peep the Horror", 120)
    {
        if (EffectHandler.Instance == null) return;
        wristText = (Text)wristTextInfo.GetValue(EffectHandler.Instance)!;
        wristImage = (Image)wristImageInfo.GetValue(EffectHandler.Instance)!;

        if (!Chaos.isSteamVer)
        {
            Il2CppOculus.Platform.Users.GetLoggedInUser().OnComplete(setName);
        }
    }

#if DEBUG
    static readonly Action<Il2CppOculus.Platform.Message<Il2CppOculus.Platform.Models.User>> setName = new(m => name = !m.IsError ? m.Data.DisplayName : throw new Exception(m.error.Message));
#endif

    readonly Dictionary<AudioSource, float> originalVolumes = new();
    readonly Dictionary<MeshRenderer, Material> originalMaterials = new();
    readonly Dictionary<SkinnedMeshRenderer, Material> originalMaterialsSkinned = new();
    [EffectPreference] static string didYouPeepIt = "DID YOU? WAS IT FUN?";
    static readonly FieldInfo wristTextInfo = typeof(EffectHandler).GetField("wristText", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new FieldAccessException("wristText");
    static readonly FieldInfo wristImageInfo = typeof(EffectHandler).GetField("wristImage", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new FieldAccessException("wristImage");
    readonly Text wristText;
    readonly Image wristImage;
    readonly AudioSource[] players = new AudioSource[4];
    object notifRoutine;
    object textRoutine;
    object imgRoutine;
    object movePlayersRoutine;
    object progressClipsRoutine;
    static AudioClip[] clips;
    static VideoPlayer videoPlayer;
    static Material mat;
    static string name;
    static readonly string[] quotes = // giygas quotes
    {
        "It hurts, {0}...",
        "...I’m h...a...p...p...y...",
        "...friends...",
        "...It hurts, ...it hurts...",
        "{0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}, {0}.....",
        "...go... b... a... c...k...",
        "....{0}...",
        "I feel... g... o... o... d...",
        "...I’m so sad... .....{0}",
        "{0}!",
        "Ah, Grrr, Ohhh...",
        "It’s not right... not right... not right...",
        "Stop... Please... stop..",
        "Please... {0}...",
    };

    [MemberNotNull(nameof(clips), nameof(mat))]
    void Init()
    {
        didYouPeepIt.ToString(); // just so the compiler doesnt yell at me for unused variable
        if (Chaos.isSteamVer)
            name = SteamApi.Name.Split(' ')[0]; // in case they have some goofball name like "fuxstik cs.money"
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    name = System.Security.Principal.WindowsIdentity.GetCurrent().Name; // https://stackoverflow.com/questions/1240373/how-do-i-get-the-current-username-in-net-using-c#1240379
        //else // name already set by static ctor, hopefully, lol.
        //    name = Environment.UserName;


        clips = GlobalVariables.ResourcePaths.Where(p => p.Contains("sounds/giygas")).OrderBy(p => p.Substring(p.Length - 3).Last()).Select(GlobalVariables.EffectResources.LoadAsset).Select(a => a.Cast<AudioClip>()).ToArray();

        videoPlayer = new GameObject("Play the horror").AddComponent<VideoPlayer>();
        videoPlayer.clip = GlobalVariables.EffectResources.LoadAsset("Assets/MemeFolder/PeepTheHorror_1.mp4").Cast<VideoClip>();
        videoPlayer.clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
        videoPlayer.playOnAwake = true;
        GameObject.DontDestroyOnLoad(videoPlayer);

        RenderTexture tex = GlobalVariables.EffectResources.LoadAsset("Assets/MaterialTextures/PeepTheHorror.renderTexture").Cast<RenderTexture>();
        mat = GlobalVariables.EffectResources.LoadAsset("Assets/Materials/PeepTheHorror.mat").Cast<Material>();
        mat.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GameObject.DontDestroyOnLoad(mat);

        mat.SetTexture(Const.URP_MAINTEX_NAME, tex);
        videoPlayer.targetTexture = tex;
        videoPlayer.isLooping = true;
    }

    public override void OnEffectStart()
    {
        if (mat == null || videoPlayer == null || clips == null || clips[0] == null) Init();

        foreach (MeshRenderer renderer in Utilities.FindAll<MeshRenderer>())
        {
            if (renderer == null) continue;
            if (renderer.transform.IsChildOfRigManager()) continue;
            originalMaterials.Add(renderer, renderer.material);
            renderer.material = mat;
        }

        foreach (SkinnedMeshRenderer renderer in Utilities.FindAll<SkinnedMeshRenderer>())
        {
            if (renderer == null) continue;
            originalMaterialsSkinned.Add(renderer, renderer.material);
            renderer.material = mat;
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new GameObject("Horror sound player " + i).AddComponent<AudioSource>();
            players[i].clip = clips[0];
            players[i].volume = 0.1f; // idgaf about your mixer shit, youre hearing this regardless.
            players[i].rolloffMode = AudioRolloffMode.Logarithmic;
            players[i].maxDistance = 15f;
            players[i].minDistance = 0.01f;
            players[i].playOnAwake = true;
            players[i].transform.position = GlobalVariables.Player_PhysRig.torso.rbHead.position + Random.insideUnitSphere * 20;
            players[i].Play();
        }

        foreach (AudioSource source in Utilities.FindAll<AudioSource>())
        {
            originalVolumes.Add(source, source.volume);
            source.volume = 0f;
        }

        notifRoutine = MelonCoroutines.Start(SendNotifications());
        textRoutine = MelonCoroutines.Start(ChangeText());
        imgRoutine = MelonCoroutines.Start(FlickerImage());
        movePlayersRoutine = MelonCoroutines.Start(MovePlayers());
        progressClipsRoutine = MelonCoroutines.Start(ProgressClips());
    }

    public override void OnEffectEnd()
    {
        MelonCoroutines.Stop(notifRoutine);
        MelonCoroutines.Stop(textRoutine);
        MelonCoroutines.Stop(imgRoutine);
        MelonCoroutines.Stop(movePlayersRoutine);
        MelonCoroutines.Stop(progressClipsRoutine);

        foreach (KeyValuePair<MeshRenderer, Material> kv in originalMaterials) kv.Key.material = kv.Value;
        foreach (KeyValuePair<SkinnedMeshRenderer, Material> kv in originalMaterialsSkinned) kv.Key.material = kv.Value;
        foreach (AudioSource player in players) player.gameObject.Destroy();
    }

    IEnumerator SendNotifications()
    {
        yield return null;
        while (Active)
        {
            string formatted = string.Format(quotes.Random(), name);
#if !NOBONELIB
            GameObject popup = PopupBoxManager.CreateNewPopupBox(formatted);
            yield return null;
            var rb = popup.GetComponentInChildren<Rigidbody>();
            rb.detectCollisions = false;
            rb.useGravity = false;
            rb.velocity = new Vector3(0, -0.1f, 0);
#endif

            //NotificationData notif = Notifications.SendNotification(formatted, Random.Range(10, 15));
            yield return new WaitForSecondsRealtime(Random.Range(10, 15) * Random.value);
#if !NOBONELIB
            popup.Destroy();
#endif
            //notif.End();
            yield return new WaitForSecondsRealtime(Random.Range(5, 20));
        }
    }

    IEnumerator FlickerImage()
    {
        yield return null;
        while (Active)
        {
            wristImage.fillAmount = Random.value;
            yield return new WaitForSecondsRealtime(Random.value * 0.5f);
        }
    }

    IEnumerator ChangeText()
    {
        yield return null;
        while (Active)
        {
            wristText.text = string.Format(quotes.Random(), name);
            yield return new WaitForSecondsRealtime(Random.value);
        }
    }

    IEnumerator MovePlayers()
    {
        yield return null;
        while (Active)
        {
            foreach (AudioSource player in players)
            {
                Vector3 randomVec = Random.insideUnitSphere;
                randomVec.y *= 0.125f;
                player.transform.position = GlobalVariables.Player_PhysRig.torso.rbHead.position + randomVec * 20;
                yield return new WaitForSecondsRealtime(Random.value * 3);
            }
        }
    }

    IEnumerator ProgressClips()
    {
        yield return null;
        float timeBetweenClips = (float)Duration / clips.Length;

        //yield return new WaitForSecondsRealtime(timeBetweenClips); // first one is already set
        for (int i = 0; i < clips.Length; i++) // first is already set
        {
#if DEBUG
            Log($"Moving to next clip at idx {i} - {clips[i].name}");
#endif
            videoPlayer.playbackSpeed = 4 * ((float)i / clips.Length);
            float clipStartTime = Time.realtimeSinceStartup;
            foreach (AudioSource player in players)
            {
                player.volume = 0.02f;
                yield return new WaitForSecondsRealtime(0.125f);
                player.volume = 0.005f;
                yield return new WaitForSecondsRealtime(0.125f);
                player.Stop();
                player.clip = clips[i];
                player.time = Time.realtimeSinceStartup - clipStartTime;
#if DEBUG
                Log($"Starting {player.gameObject.name} at {Time.realtimeSinceStartup - clipStartTime} ({Time.realtimeSinceStartup} - {clipStartTime})");
#endif
                player.Play();
                yield return new WaitForSecondsRealtime(0.125f);
                player.volume = 0.02f;
                yield return new WaitForSecondsRealtime(0.125f);
                player.volume = 0.1f;
            }

#if DEBUG
            Log($"Waiting for {timeBetweenClips - players.Length * 0.5f} seconds");
#endif
            yield return new WaitForSecondsRealtime(timeBetweenClips - players.Length * 0.5f);
        }
    }
}
