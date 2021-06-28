using System;
using System.Collections;

using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class Lag : EffectBase
    {
        public Lag() : base("Lag", 30) { }

        public override void OnEffectStart()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
            MelonCoroutines.Start(DoLag());
        }

        private IEnumerator DoLag()
        {
            // todo: using Epsilon instead of 0 might work better
            while (active)
            {
                yield return new WaitForSeconds(3 / UnityEngine.Random.value);
                Time.timeScale = Time.timeScale == 1 ? 0 : 1;
            }
        }

        public override void OnEffectEnd()
            => GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
    }
}
