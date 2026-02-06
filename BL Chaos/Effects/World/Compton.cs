using System.Diagnostics.CodeAnalysis;
using Jevil.Patching;

namespace BLChaos.Effects;

internal class Compton : EffectBase
{
    static AudioClip[] clips = Array.Empty<AudioClip>();
    static bool active = false;
    public Compton() : base("Compton", 30) { Init(); }

    [MemberNotNull(nameof(clips))]
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
        if (clips.Length == 0 || clips[0] == null) Init();
        #endregion

        active = true;

        GameCallbacks.OnPreAudioSourcePlay += ReplaceClip;
    }

    public override void OnEffectEnd()
    {
        active = false;
        GameCallbacks.OnPreAudioSourcePlay -= ReplaceClip;
    }

    // basically yoinked from AudioReplacer (https://github.com/TrevTV/Boneworks-OpenSourceMods/blob/main/AudioReplacer/MelonLoaderMod.cs lines 61, 62)
    static void ReplaceClip(AudioSource instance)
    {
        instance.clip = clips.Random();
    }
}
