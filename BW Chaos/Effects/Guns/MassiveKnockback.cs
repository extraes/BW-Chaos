using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class MassiveKnockback : EffectBase
    {
        public MassiveKnockback() : base("Barry Steakfries", 30) { }

        public override void OnEffectStart()
        {
            Hooking.OnPreFireGun += GunFirePre;
            Hooking.OnPostFireGun += GunFirePost;
        }

        private void GunFirePre(Gun gun)
        {
            gun.kickForce *= 100;
            gun.magazineSocket.isInfiniteAmmo = true;
        }

        private void GunFirePost(Gun gun)
        {
            gun.kickForce /= 100;
            gun.magazineSocket.isInfiniteAmmo = false;
        }

        public override void OnEffectEnd()
        {
            Hooking.OnPreFireGun -= GunFirePre;
            Hooking.OnPostFireGun -= GunFirePost;
        }
    }
}
