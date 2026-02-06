#if !NOBONELIB
namespace BLChaos.Effects;

internal class ForceGunManual : EffectBase
{
    public ForceGunManual() : base("Make held gun(s) manual") { }

    public override void OnEffectStart()
    {
        Gun g = Player.GetComponentInHand<Gun>(Player.RightHand);
        if (g != null)
        {
            g.fireMode = Gun.FireMode.MANUAL;
            g.slideState = Gun.SlideStates.LOCKED;
        }

        g = Player.GetComponentInHand<Gun>(Player.LeftHand);
        if (g != null)
        {
            g.fireMode = Gun.FireMode.MANUAL;
            g.slideState = Gun.SlideStates.LOCKED;
        }
    }

}
#endif