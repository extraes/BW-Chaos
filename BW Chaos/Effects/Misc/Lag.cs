using System;
using System.Collections;

using UnityEngine;
using MelonLoader;
using Random = UnityEngine.Random;
using System.Linq;

namespace BWChaos.Effects
{
    internal class Lag : EffectBase
    {
        public Lag() : base("Lag", 60, EffectTypes.LAGGY) { }
        [RangePreference(0, 1, 0.0625f)] static float timeScale = 0.0625f;

        public override void OnEffectStart()
        {
            Utilities.DisableSloMo();
        }

        public override void OnEffectEnd()
        {
            Utilities.EnableSloMo();
        }

        public override void HandleNetworkMessage(byte[] data)
        {
            float i1 = BitConverter.ToSingle(data, 0);
            float i2 = BitConverter.ToSingle(data, sizeof(float));
            MelonCoroutines.Start(NetLag(i1, i2));
        }

        private IEnumerator NetLag(float waitTime, float slowTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(slowTime);
            Time.timeScale = 1;
        }

        [AutoCoroutine]
        public IEnumerator DoLag()
        {
            if (isNetworked) yield break;
            while (Active)
            {
                float[] laggy = { Random.Range(1, 1.5f), Random.Range(0.25f, 0.5f) };

                SendNetworkData(laggy.Select(f => BitConverter.GetBytes(f)).ToArray().Flatten());

                // there should be intermittent stuttering, and the time between stutters should be longer than the stutters themselves
                yield return new WaitForSecondsRealtime(laggy[0]);
                Time.timeScale = timeScale; // setting to 0 doesn't break anything, but float.Epsilon certainly does, so use 1/16th speed
                yield return new WaitForSecondsRealtime(laggy[1]);
                Time.timeScale = 1;
            }
        }
    }
}
