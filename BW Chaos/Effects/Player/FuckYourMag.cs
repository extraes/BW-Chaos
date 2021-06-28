using System;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;

namespace BW_Chaos.Effects
{
    internal class FuckYourMag : EffectBase
    {
        public FuckYourMag() : base("Fuck Your Magazine", 60) { }

        public override void OnEffectUpdate()
        {
            // todo: might want a wait in between drops
            Gun gun = UnityEngine.Random.Range(0, 2) == 1
                ? Player.GetGunInHand(Player.leftHand)
                : Player.GetGunInHand(Player.rightHand);

            gun?.magazineSocket?.MagazineRelease();
        }
    }
}
