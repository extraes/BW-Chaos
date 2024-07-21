using Jevil.Patching;
using Il2CppSLZ.Bonelab;
using UnityEngine;
using Il2CppSLZ.Marrow;
using Jevil;

namespace BLChaos.Effects;

internal class FourTimesSpeed : EffectBase
{
    public FourTimesSpeed() : base("4x Speed", 30) { }

    static FourTimesSpeed() => Chaos.Instance.HarmonyInstance.Patch(Utilities.AsInfo(TimeManager.DECREASE_TIMESCALE), prefix: Utilities.ToHarmony(WhenImActive));
    static bool WhenImActive() => enabled;
    static bool enabled;

    public override void OnEffectStart() => enabled = true;
    public override void OnEffectUpdate() => Time.timeScale = 4;
    public override void OnEffectEnd()
    {
        enabled = false;
        Time.timeScale = 1;
    }
}
