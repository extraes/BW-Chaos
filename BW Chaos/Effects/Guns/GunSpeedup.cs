using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    class GunSpeedup : EffectBase
    {
        // If a gun gets too fast, the pooled bullets will be reused before they can hit anything... Oops!
        public GunSpeedup() : base("Progressively faster guns", 60) { }

        public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
        public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

        public void OnGunFired (StressLevelZero.Props.Weapons.Gun gun)
        {
            if (gun == Player.GetGunInHand(Player.rightHand) || gun == Player.GetGunInHand(Player.leftHand))
            {
                gun.SetRpm(gun.roundsPerMinute * 1.01f);
            }
        }
    }
}
