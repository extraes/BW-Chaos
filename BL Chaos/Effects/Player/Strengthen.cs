#if !NOBONELIB
namespace BLChaos.Effects;

internal class Strengthen : EffectBase
{
    public Strengthen() : base("Live Simulation of ME (Chad)", 60) { }

    public override void OnEffectStart()
    {
        Utilities.ChangeStrength(Player.LeftHand.physHand);
        Utilities.ChangeStrength(Player.RightHand.physHand);
    }

    public override void OnEffectEnd()
    {
        Utilities.ChangeStrength(Player.LeftHand.physHand);
        Utilities.ChangeStrength(Player.RightHand.physHand);
    }
}
#endif