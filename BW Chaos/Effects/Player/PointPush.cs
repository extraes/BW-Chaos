using ModThatIsNotMod;
using UnityEngine;

namespace BWChaos.Effects;

internal class PointPush : EffectBase
{
    public PointPush() : base("Point Push", 30) { }
    [RangePreference(0, 10, 0.125f)] static readonly float forceMultiplier = 1;

    public override void OnEffectUpdate()
        => GlobalVariables.Player_PhysBody.AddImpulseForce(Player.rightHand.transform.forward.normalized * 1500f * forceMultiplier * Time.deltaTime);
    // unscaled 250f is too much when this effect runs once a frame
}
