using System.Reflection;

namespace BWChaos.Effects;

internal class FastTimer : EffectBase
{
    public FastTimer() : base("Fast Effect Timer", 90, EffectTypes.META) { }

    public override void OnEffectStart() => EffectHandler.Instance.secondsEachEffect /= 2;

    public override void OnEffectEnd() => EffectHandler.Instance.secondsEachEffect = Const.DEFAULT_SEC_EACH_EFFECT;
}
