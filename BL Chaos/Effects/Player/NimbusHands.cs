#if !NOBONELIB
namespace BLChaos.Effects;

internal class NimbusHands : EffectBase
{
    public NimbusHands() : base("Nimbus Hands", 30) { }
    [RangePreference(0, 10, 0.25f)] static float forceMultiplier = 0.5f;

    public override void OnEffectStart()
    {
        Utilities.ChangeStrength(Player.LeftHand.physHand, 100, 5);
        Utilities.ChangeStrength(Player.RightHand.physHand, 100, 5);
    }
    public override void OnEffectEnd()
    {
        Utilities.ChangeStrength(Player.LeftHand.physHand);
        Utilities.ChangeStrength(Player.RightHand.physHand);
    }
    public override void OnEffectUpdate()
    {
        // only do it to one hand at a time because framerate is high enough to make it not matter
        Hand hand = Time.frameCount % 2 == 0 ? Player.LeftHand : Player.RightHand;
        Vector3 vel = hand.rb.velocity;

        // make sure the velocity is velocity relative to the body
        vel -= GlobalVariables.Player_PhysRig.torso.rbPelvis.velocity;

        vel *= forceMultiplier * Time.deltaTime * 100; // effectively square it to make smaller movements not move as much

        vel.x *= 0.5f;
        vel.z *= 0.5f;

        GlobalVariables.Player_PhysRig.AddVelocityChange(-Vector3.ClampMagnitude(vel, 5 * forceMultiplier));
    }
}
#endif