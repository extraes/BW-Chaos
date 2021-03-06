using ModThatIsNotMod;

namespace BWChaos.Effects;

internal class ForceGunManual : EffectBase
{
    public ForceGunManual() : base("Make held gun(s) manual") { }

    public override void OnEffectStart()
    {
        StressLevelZero.Props.Weapons.Gun g = Player.GetGunInHand(Player.rightHand);
        if (g != null)
        {
            g.isManual = true;
            g.isAutomatic = false;
            g.slideState = StressLevelZero.Props.Weapons.Gun.SlideStates.LOCKED;
        }

        g = Player.GetGunInHand(Player.leftHand);
        if (g != null)
        {
            g.isManual = true;
            g.isAutomatic = false;
            g.slideState = StressLevelZero.Props.Weapons.Gun.SlideStates.LOCKED;
        }
    }

}
