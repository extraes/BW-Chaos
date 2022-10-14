using System.Reflection;

namespace BLChaos.Effects;

internal class SlowTimer : EffectBase
{
    public SlowTimer() : base("Slow Effect Timer", 135, EffectTypes.META) { }

    public override void OnEffectStart() => EffectHandler.Instance.secondsEachEffect *= 2;

    public override void OnEffectEnd() => EffectHandler.Instance.secondsEachEffect = Const.DEFAULT_SEC_EACH_EFFECT;
}
