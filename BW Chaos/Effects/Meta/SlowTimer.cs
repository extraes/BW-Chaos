using System.Reflection;

namespace BWChaos.Effects;

internal class SlowTimer : EffectBase
{
    public SlowTimer() : base("Slow Effect Timer", 135, EffectTypes.META) { }
    static readonly FieldInfo secInfo = typeof(EffectHandler).GetField("secondsEachEffect", BindingFlags.Instance | BindingFlags.NonPublic);

    public override void OnEffectStart() => secInfo.SetValue(EffectHandler.Instance, 45);

    public override void OnEffectEnd() => secInfo.SetValue(EffectHandler.Instance, 30);
}
