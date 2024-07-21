using BoneLib;
using Jevil;

namespace BLChaos.Effects;

internal class Strengthen : EffectBase
{
    public Strengthen() : base("Live Simulation of ME (Chad)", 60) { }

    public override void OnEffectStart()
    {
        Utilities.ChangeStrength(Player.leftHand.physHand);
        Utilities.ChangeStrength(Player.rightHand.physHand);
    }

    public override void OnEffectEnd()
    {
        Utilities.ChangeStrength(Player.leftHand.physHand);
        Utilities.ChangeStrength(Player.rightHand.physHand);
    }
}
