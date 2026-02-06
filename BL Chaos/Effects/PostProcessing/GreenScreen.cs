using BoneLib;
using Il2Cpp;
using Jevil.PostProcessing;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Video;

namespace BLChaos.Effects;

internal class GreenScreen : EffectBase
{
    public GreenScreen() : base("GreenScreen", 15, EffectTypes.POST_PROCESS) { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    static GreenScreen()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        GameObject go = new("Chaos Video Player - GreenScreen");
        player = go.AddComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.isLooping = false;
    
        player.targetTexture = rTex;
        player.source = VideoSource.VideoClip;
        player.renderMode = VideoRenderMode.RenderTexture;

        go.Persist();
        player.Persist();
        rTex.Persist();
        player.prepareCompleted += (new System.Action<VideoPlayer>(_ => ShowOverlay()));
        player.loopPointReached += (new System.Action<VideoPlayer>(_ => SharedPostProcessingMaterials.GreenScreen.Disable()));
    }

    static string[] paths = GlobalVariables.ResourcePaths.Where(p => p.ToLower().Contains("assets/greenscreens/memes")).ToArray();
    static VideoClip[] clips;
    static VideoPlayer player;
    static AudioSource aSource;
    static RenderTexture rTex = new(512, 512, 0);
    static GlobalTextureDescriptor globalDesc = new("_AltTex", rTex);

    static Dictionary<string, Color> backingColors = new() // because greenscreen apparently doesnt mean GREEN (0,255,0)
    {
        { "cat rizz", new Color32(0, 214, 0, 0) },
        { "chicken", new Color32(1, 161, 0, 0) },
        { "gibby" , new Color32(1, 214, 2, 0) },
        { "here we go again" , new Color32(0, 162, 1, 0) },
        { "meow" , new Color32(1, 161, 0, 0) },
    };

    [MemberNotNull(nameof(clips))]
    static void Init()
    {
        clips = paths.Select(GlobalVariables.EffectResources.LoadAsset).Select(clip => clip.Cast<VideoClip>()).ToArray();

    }

    public override void OnEffectStart()
    {
        if (clips is null || clips.Length == 0 || clips[0] == null)
        {
            Init();
        }

        if (aSource == null)
        {
            aSource = GlobalVariables.Player_PhysRig.m_head.gameObject.AddComponent<AudioSource>();
            player.SetTargetAudioSource(0, aSource);
            aSource.volume = 0.4f;
            aSource.outputAudioMixerGroup = Audio.InHead;
            aSource.Persist();
        }

        player.clip = null;
        player.clip = clips.Random();

        player.Prepare();
        PostProcessingManager.SetGlobalTexture(globalDesc);
    }

    public override void OnEffectEnd()
    {
        player.Stop();
        PostProcessingManager.RemoveGlobalTexture(globalDesc);
        SharedPostProcessingMaterials.GreenScreen.Disable(); // in case the videoplayer never gets to finish
        SharedPostProcessingMaterials.GreenScreen.KeyColor.ResetOn(SharedPostProcessingMaterials.GreenScreen.Material);
        SharedPostProcessingMaterials.GreenScreen.Tolerance.ResetOn(SharedPostProcessingMaterials.GreenScreen.Material);
    }

    static void ShowOverlay()
    {
        player.Play();
        SharedPostProcessingMaterials.GreenScreen.Enable();
        if (backingColors.TryGetValue(player.clip.name, out Color keyColor))
            SharedPostProcessingMaterials.GreenScreen.KeyColor.SetOn(SharedPostProcessingMaterials.GreenScreen.Material, keyColor);
        SharedPostProcessingMaterials.GreenScreen.Tolerance.SetOn(SharedPostProcessingMaterials.GreenScreen.Material, 0.4f);
    }
}
