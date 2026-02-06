#if !NOBONELIB
namespace BLChaos.Effects;

internal class Weaken : EffectBase
{
    public Weaken() : base("What 0 Pussy Does to a MF", 15) { }

    public override void OnEffectStart()
    {
        //GlobalVariables.Player_RigManager.physicsRig.EnableBallLoco();
        Utilities.ChangeStrength(Player.LeftHand.physHand, 0.2f, 0.2f);
        Utilities.ChangeStrength(Player.RightHand.physHand, 0.2f, 0.2f);
    }

    public override void OnEffectEnd()
    {
        //GlobalVariables.Player_RigManager.physicsRig.DisableBallLoco();
        Utilities.ChangeStrength(Player.LeftHand.physHand, 1, 1);
        Utilities.ChangeStrength(Player.RightHand.physHand, 1, 1);
    }
}
#endif