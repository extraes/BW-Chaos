using Jevil.Patching;

namespace BLChaos.Effects;

internal class VineBoomSounds : EffectBase
{
    static AudioClip vineBoomSound;
    static bool active = false;
    public VineBoomSounds() : base("Vine Boom Sound Effects", 30) { Init(); }

    private void Init()
    {
        vineBoomSound = GlobalVariables.EffectResources.LoadAsset(GlobalVariables.ResourcePaths.FirstOrDefault(p => p.ToLower().Contains("vineboom"))).Cast<AudioClip>();
        vineBoomSound.hideFlags = HideFlags.DontUnloadUnusedAsset;

#if DEBUG
        Log("Loaded the moyai sound into VineBoomSounds");
        if (vineBoomSound == null) Chaos.Error("Scratch that, it's null. Blame the IRS. And the CIA, those bioluminescent fucks");
#endif
    }

    public override void OnEffectStart()
    {
        if (vineBoomSound == null) Init();

        active = true;

        GameCallbacks.OnPreAudioSourcePlay += Replace;
    }

    public override void OnEffectEnd()
    {
        GameCallbacks.OnPreAudioSourcePlay -= Replace;

        active = false;
    }

    // basically yoinked from AudioReplacer (https://github.com/TrevTV/Boneworks-OpenSourceMods/blob/main/AudioReplacer/MelonLoaderMod.cs lines 61, 62)
    static void Replace(AudioSource __instance)
    {
        if (vineBoomSound != null)
            __instance.clip = vineBoomSound;
    }
    
}
