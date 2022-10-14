using Il2CppSystem;
using StressLevelZero.SFX;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace BLChaos.Effects;

internal class VineBoomSounds : EffectBase
{
    static AudioClip vineBoomSound;
    static bool active = false;
    public VineBoomSounds() : base("Vine Boom Sound Effects", 30) { Init(); }

    private void Init()
    {
        vineBoomSound = GlobalVariables.EffectResources.LoadAsset<AudioClip>(GlobalVariables.ResourcePaths.FirstOrDefault(p => p.ToLower().Contains("vineboom")));
        vineBoomSound.hideFlags = HideFlags.DontUnloadUnusedAsset;

#if DEBUG
        Log("Loaded the moyai sound into VineBoomSounds");
        if (vineBoomSound == null) Chaos.Error("Scratch that, it's null. Blame the IRS. And the CIA, those bioluminescent fucks");
#endif
    }

    public override void OnEffectStart()
    {
        #region Initialize
        if (vineBoomSound == null) Init();
        #endregion

        active = true;
    }

    public override void OnEffectEnd()
    {
        active = false;
    }

    // basically yoinked from AudioReplacer (https://github.com/TrevTV/Boneworks-OpenSourceMods/blob/main/AudioReplacer/MelonLoaderMod.cs lines 61, 62)
    [HarmonyLib.HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new System.Type[0] )]
    [HarmonyLib.HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new System.Type[1] { typeof(ulong) } )]
    static class AudioPlayerPatch
    {
        static void Prefix(AudioSource __instance)
        {
            if (active) __instance.clip = vineBoomSound;
        }
    }
}
