using BoneLib;
using UnityEngine;

namespace BLChaos.Effects;

internal class PointGravity : EffectBase
{
    public PointGravity() : base("Point Gravity", 30, EffectTypes.AFFECT_GRAVITY | EffectTypes.DONT_SYNC) { }
    [RangePreference(0, 10, 0.25f)] static readonly float gravityMultiplier = 1f;

    public override void OnEffectUpdate() => Physics.gravity = Player.rightHand.transform.forward.normalized * 12f * gravityMultiplier;

    public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
}
