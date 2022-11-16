using Jevil;

namespace BLChaos.Effects;

internal class InvertPlayerVelocity : EffectBase
{
    public InvertPlayerVelocity() : base("Invert player velocity") { }

    public override void OnEffectStart() =>
        GlobalVariables.Player_PhysRig.AddVelocityChange(-2 * GlobalVariables.Player_PhysRig.torso.rbPelvis.velocity);

}
