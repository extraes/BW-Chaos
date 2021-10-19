using System;
using System.Collections;

using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class Lag : EffectBase
    {
        public Lag() : base("Lag", 60, EffectTypes.LAGGY) { }

        public override void OnEffectStart()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
            MelonCoroutines.Start(DoLag());
        }

        private IEnumerator DoLag()
        {
            while (Active)
            {
                // there should be intermittent stuttering, and the time between stutters should be longer than the stutters themselves
                yield return new WaitForSeconds(UnityEngine.Random.Range(1, 1.5f));
                Time.timeScale = 0.0625f; // setting to 0 doesn't break anything, but float.Epsilon certainly does
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.5f));
                Time.timeScale = 1;
            }
        }

        public override void OnEffectEnd()
            => GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
    }
}
