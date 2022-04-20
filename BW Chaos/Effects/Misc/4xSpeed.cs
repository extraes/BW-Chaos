using UnityEngine;

namespace BWChaos.Effects;

internal class FourTimesSpeed : EffectBase
{
    public FourTimesSpeed() : base("4x Speed", 30) { }

    public override void OnEffectStart() => Utilities.DisableSloMo();
    public override void OnEffectUpdate() => Time.timeScale = 4;
    public override void OnEffectEnd()
    {
        Utilities.EnableSloMo();
        Time.timeScale = 1;
    }
}
