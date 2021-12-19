using System;
using System.Collections;

using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class SlowShooting : EffectBase
    {
        public SlowShooting() : base("SUPERSHOT", 90) { }

        private object coroutine;

        public override void OnEffectStart()
        {
            Hooking.OnPostFireGun += OnPostGunFire;
        }

        private void OnPostGunFire(Gun obj)
        {
            // avoid conflicting coroutines
            if (coroutine != null) MelonCoroutines.Stop(coroutine);
            coroutine = MelonCoroutines.Start(BringBackTime());
        }

        private IEnumerator BringBackTime()
        {
            Time.timeScale = 0.05f;
            while (Time.timeScale < 1f && Active)
            {
                Time.timeScale += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
            Time.timeScale = 1;
        }

        public override void OnEffectEnd()
        {
            Hooking.OnPostFireGun -= OnPostGunFire;
        }
    }
}
