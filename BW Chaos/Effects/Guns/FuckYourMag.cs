using System;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class FuckYourMag : EffectBase
    {
        public FuckYourMag() : base("Fuck Your Magazines") { }

        public override void OnEffectStart()
        {
            Gun gun = Player.GetGunInHand(Player.leftHand);
            gun?.magazineSocket?.MagazineRelease();
            gun = Player.GetGunInHand(Player.rightHand);
            gun?.magazineSocket?.MagazineRelease();
        }
    }
}
