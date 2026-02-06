#if !NOBONELIB
namespace BLChaos.Effects;

internal class GravityHands : EffectBase
{
    public GravityHands() : base("Gravity Hands", 30, EffectTypes.AFFECT_GRAVITY | EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
    static readonly float gravityMultiplier = 4f;

    public override void OnEffectUpdate() => Physics.gravity = (Player.LeftHand.rb.velocity + Player.RightHand.rb.velocity - 2 * GlobalVariables.Player_PhysRig.torso.rbPelvis.velocity) * gravityMultiplier;
}
#endif