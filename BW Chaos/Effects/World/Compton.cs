using Il2CppSystem;
using StressLevelZero.SFX;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace BWChaos.Effects;

internal class Compton : EffectBase
{
    static AudioClip[] clips;
    static bool active = false;
    public Compton() : base("Compton", 30) { Init(); }

    private void Init()
    {
        clips = Resources.FindObjectsOfTypeAll<AudioClip>().Where(c => c.name.ToLower().Contains("gunshot")).ToArray();
        foreach (AudioClip clip in clips) clip.hideFlags = HideFlags.DontUnloadUnusedAsset;

#if DEBUG
        Log("Got our clips! They are:");
        foreach (AudioClip clip in clips) Log("   " + clip.name);
#endif
    }

    public override void OnEffectStart()
    {
        #region Initialize
        if (clips == null || clips.Length == 0 || clips[0] == null) Init();
        #endregion

        active = true;
    }

    public override void OnEffectEnd()
    {
        active = false;
    }

    // basically yoinked from AudioReplacer (https://github.com/TrevTV/Boneworks-OpenSourceMods/blob/main/AudioReplacer/MelonLoaderMod.cs lines 61, 62)
    [HarmonyLib.HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new System.Type[0])]
    [HarmonyLib.HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new System.Type[1] { typeof(ulong) })]
    static class AudioPlayerPatch
    {
        static void Prefix(AudioSource __instance)
        {
            if (active) __instance.clip = clips.Random();
        }
    }
}
