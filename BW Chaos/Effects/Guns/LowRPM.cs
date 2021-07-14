using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class LowRPM : EffectBase
    {
        public LowRPM() : base("When No VTEC", 30) { }

        public override void OnEffectStart()
        {
            Hooking.OnPreFireGun += GunFirePre;
            Hooking.OnPostFireGun += GunFirePost;
        }

        private void GunFirePre(Gun gun) => gun.SetRpm(gun.roundsPerMinute / 25);

        private void GunFirePost(Gun gun) => gun.SetRpm(gun.roundsPerMinute * 25);

        public override void OnEffectEnd()
        {
            Hooking.OnPreFireGun -= GunFirePre;
            Hooking.OnPostFireGun -= GunFirePost;
        }
    }
}
