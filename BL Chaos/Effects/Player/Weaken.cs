using BoneLib;
using Jevil;

namespace BLChaos.Effects;

internal class Weaken : EffectBase
{
    public Weaken() : base("What 0 Pussy Does to a MF", 15) { }

    public override void OnEffectStart()
    {
        //GlobalVariables.Player_RigManager.physicsRig.EnableBallLoco();
        Utilities.ChangeStrength(Player.leftHand.physHand, (1 / 5f), (1 / 5f));
        Utilities.ChangeStrength(Player.rightHand.physHand, (1 / 5f), (1 / 5f));
    }

    public override void OnEffectEnd()
    {
        //GlobalVariables.Player_RigManager.physicsRig.DisableBallLoco();
        Utilities.ChangeStrength(Player.leftHand.physHand, 1, 1);
        Utilities.ChangeStrength(Player.rightHand.physHand, 1, 1);
    }
}
