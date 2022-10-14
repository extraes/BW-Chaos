using UnityEngine;

namespace BLChaos.Effects;

internal class ExtremeGravity : EffectBase
{
    public ExtremeGravity() : base("Extreme Gravity", 30, EffectTypes.AFFECT_GRAVITY) { }

    public override void OnEffectStart() => Physics.gravity = Vector3.down * 50;

    public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
}
