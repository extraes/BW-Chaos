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
        public SlowShooting() : base("Slow Shooting", 30) { }

        private IEnumerator coroutine;

        public override void OnEffectStart()
        {
            Hooking.OnPostFireGun += OnPostGunFire;
        }

        private void OnPostGunFire(Gun obj)
        {
            if (coroutine != null) MelonCoroutines.Stop(coroutine);
            coroutine = (IEnumerator)MelonCoroutines.Start(BringBackTime());
        }

        private IEnumerator BringBackTime()
        {
            // todo: wait might want to be changed
            Time.timeScale = 0.05f;
            while (Time.timeScale < 1f && Active)
            {
                Time.timeScale += 0.05f;
                yield return new WaitForSeconds(0.1f);
            }
            Time.timeScale = 1;
        }

        public override void OnEffectEnd()
        {
            Hooking.OnPostFireGun -= OnPostGunFire;
        }
    }
}
