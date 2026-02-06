using Il2CppMK.Glow.URP;

namespace BLChaos.Effects;

internal class E32016 : EffectBase
{
    public E32016() : base("E3 2016", 30, EffectTypes.POST_PROCESS) { }

    record struct BloomState(float BloomIntensity);

    MKGlow[] glowies; // NSA wsg
    BloomState[] states;
    public override void OnEffectStart()
    {
        if (Utilities.IsPlatformQuest())
        {
            Utilities.SpawnAd("E3 2016 didn't happen on quest. Womp womp.");
            return;
        }

        glowies = GameObject.FindObjectsOfType<MKGlow>();
        states = new BloomState[glowies.Length];

        for (int i = 0; i < glowies.Length; i++)
        {
            states[i] = new BloomState(glowies[i].bloomIntensity.value);
            glowies[i].bloomIntensity.Override(10f);
        }
    }

    public override void OnEffectEnd()
    {
        for (int i = 0; i < glowies.Length; i++)
        {
            if (glowies[i] == null)
                continue;
            glowies[i].bloomIntensity.Override(states[i].BloomIntensity);
            // use if needed: glowies[i].bloomIntensity.overrideState = false;
        }
    }
}
