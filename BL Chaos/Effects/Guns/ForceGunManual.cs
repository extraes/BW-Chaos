

using BoneLib;
using SLZ.Props.Weapons;

namespace BLChaos.Effects;

internal class ForceGunManual : EffectBase
{
    public ForceGunManual() : base("Make held gun(s) manual") { }

    public override void OnEffectStart()
    {
        Gun g = Player.GetGunInHand(Player.rightHand);
        if (g != null)
        {
            g.fireMode = Gun.FireMode.MANUAL;
            g.slideState = Gun.SlideStates.LOCKED;
        }

        g = Player.GetGunInHand(Player.leftHand);
        if (g != null)
        {
            g.fireMode = Gun.FireMode.MANUAL;
            g.slideState = Gun.SlideStates.LOCKED;
        }
    }

}
