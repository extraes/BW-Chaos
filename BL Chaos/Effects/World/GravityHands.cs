using BoneLib;
using UnityEngine;

namespace BLChaos.Effects;

internal class GravityHands : EffectBase
{
    public GravityHands() : base("Gravity Hands", 30, EffectTypes.AFFECT_GRAVITY | EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
    static readonly float gravityMultiplier = 4f;

    public override void OnEffectUpdate() => Physics.gravity = (Player.leftHand.rb.velocity + Player.rightHand.rb.velocity - 2 * GlobalVariables.Player_PhysRig.torso.rbPelvis.velocity) * gravityMultiplier;
}
