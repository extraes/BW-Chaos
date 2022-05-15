using UnityEngine;

namespace BWChaos.Effects;

internal class InvertGravity : EffectBase
{
    public InvertGravity() : base("Invert Gravity", 45, EffectTypes.AFFECT_GRAVITY) { }

    public override void OnEffectStart() => Physics.gravity = new Vector3(0, 9.81f, 0);

    public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
}
