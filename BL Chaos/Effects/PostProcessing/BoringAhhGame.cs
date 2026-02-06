using Jevil.PostProcessing;
using UnityEngine.Video;

namespace BLChaos.Effects;

internal class BoringAhhGame : EffectBase
{
    public BoringAhhGame() : base("Boring Ahh Game", 30, EffectTypes.POST_PROCESS) { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    static BoringAhhGame()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        if (aSource == null)
        {
            aSource = GlobalVariables.Player_PhysRig.m_head.gameObject.AddComponent<AudioSource>();
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
        PostProcessingManager.RemoveGlobalTexture(globalDesc);
        SharedPostProcessingMaterials.SideScreen.Disable();
    }
}
