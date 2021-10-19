using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    class GunSlowdown : EffectBase
    {
        public GunSlowdown() : base("Progressively slower guns", 60) { }

        public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
        public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

        public void OnGunFired (StressLevelZero.Props.Weapons.Gun gun)
        {
            if (gun == Player.GetGunInHand(Player.rightHand) || gun == Player.GetGunInHand(Player.leftHand))
            {
                gun.SetRpm(gun.roundsPerMinute * 0.99f);
            }
        }
    }
}
