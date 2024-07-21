using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Jevil.PostProcessing;
using Random = UnityEngine.Random;
using Jevil;
using UnityEngine.Video;
using System.IO;
using BoneLib;

namespace BLChaos.Effects;

internal class BoringAhhGame : EffectBase
{
    public BoringAhhGame() : base("Boring Ahh Game", 30, EffectTypes.POST_PROCESS) { }
    static BoringAhhGame()
    {
        string tmpDir = Path.GetTempPath();
        string filePath = Path.Combine(tmpDir, "Subway Surfers.mp4");
        if (!File.Exists(filePath))
            Chaos.Assembly.UseEmbeddedResource("BLChaos.Resources.SubwaySurfers.mp4", bytes => File.WriteAllBytes(filePath, bytes));

        GameObject go = new("Chaos Video Player - Boring Ahh Game");
        player = go.AddComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.url = filePath;
        player.targetTexture = rTex;
        player.source = VideoSource.Url;
        player.renderMode = VideoRenderMode.RenderTexture;

        
        go.Persist();
        player.Persist();
        rTex.Persist();
    }
    static VideoPlayer player;
    static AudioSource aSource;
    static RenderTexture rTex = new(256, 512, 0);
    static GlobalTextureDescriptor globalDesc = new("_AltTex", rTex);

    public override void OnEffectStart()
    {
        if (aSource.INOC())
        {
            aSource = Player.playerHead.gameObject.AddComponent<AudioSource>();
            player.SetTargetAudioSource(0, aSource);
            aSource.volume = 0.4f;
            aSource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
            aSource.Persist();
        }

        PostProcessingManager.SetGlobalTexture(globalDesc);
        Material matt = SharedPostProcessingMaterials.SideScreen.Material;
        SharedPostProcessingMaterials.SideScreen.Enable();
        player.Play();
    }

    public override void OnEffectEnd()
    {
        player.Stop();
        SharedPostProcessingMaterials.SideScreen.Disable();
    }
}
