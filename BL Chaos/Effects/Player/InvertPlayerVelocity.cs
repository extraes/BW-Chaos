namespace BLChaos.Effects;

internal class InvertPlayerVelocity : EffectBase
{
    public InvertPlayerVelocity() : base("Invert player velocity") { }

    public override void OnEffectStart() =>
        GlobalVariables.Player_PhysBody.AddVelocityChange(-2 * GlobalVariables.Player_PhysBody.rbPelvis.velocity);

}
