using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class RandomTimeScale : EffectBase
    {
        public RandomTimeScale() : base("Random slowmo", 90) { }

        
        public override void OnEffectEnd()
        {
            Time.timeScale = 1;
        }

        [Extras.AutoCoroutine]
        public IEnumerator ChangeTime()
        {
            yield return new WaitForSecondsRealtime(Random.RandomRange(6, 10));
            if (!Active) yield break;

            var times = new float[] { 0.125f, 0.25f, 0.5f };

            Time.timeScale = times[Random.RandomRange(0, times.Length)];
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1;

            MelonCoroutines.Start(ChangeTime());
        }
    }
}
