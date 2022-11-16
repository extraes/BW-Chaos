
using BoneLib;
using Jevil;
using UnityEngine;

namespace BLChaos.Effects;

internal class PointPush : EffectBase
{
    public PointPush() : base("Point Push", 30) { }
    [RangePreference(0, 10, 0.25f)] static readonly float forceMultiplier = 1;

    public override void OnEffectUpdate()
        => GlobalVariables.Player_PhysRig.AddVelocityChange(5 * forceMultiplier * Time.deltaTime * Player.rightHand.transform.forward);
    // unscaled 250f is too much when this effect runs once a frame
}
