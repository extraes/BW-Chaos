using UnityEngine;

namespace BLChaos.Effects;

internal class ZeroGravity : EffectBase
{
    public ZeroGravity() : base("Zero Gravity", 90, EffectTypes.AFFECT_GRAVITY) { }

    private Vector3 previousGrav;
    private Vector3 zeroGrav = new Vector3(0f, -0.001f, 0f);

    public override void OnEffectStart()
    {
        previousGrav = Physics.gravity;
        Physics.gravity = zeroGrav;
    }

    public override void OnEffectEnd() => Physics.gravity = previousGrav;
}
