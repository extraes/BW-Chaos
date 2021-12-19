using System;
using System.Collections;

using UnityEngine;
using MelonLoader;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class Lag : EffectBase
    {
        public Lag() : base("Lag", 60, EffectTypes.LAGGY) { }
        public (float, float) laggy;
        public override void OnEffectStart()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
        }

        public override void OnEffectEnd()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
        }

        public override void HandleNetworkMessage(string data)
        {
            string[] datas = data.Split(',');
            float i1 = float.Parse(datas[0]);
            float i2 = float.Parse(datas[1]);
            MelonCoroutines.Start(NetLag(i1, i2));
        }

        private IEnumerator NetLag(float waitTime, float slowTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Time.timeScale = 0.0625f;
            yield return new WaitForSecondsRealtime(slowTime);
            Time.timeScale = 1;
        }

        [AutoCoroutine]
        public IEnumerator DoLag()
        {
            if (isNetworked) yield break;
            while (Active)
            {
                laggy = (Random.Range(1, 1.5f), Random.Range(0.25f, 0.5f));
                SendNetworkData(laggy.Item1 + "," + laggy.Item2);
                // there should be intermittent stuttering, and the time between stutters should be longer than the stutters themselves
                yield return new WaitForSecondsRealtime(laggy.Item1);
                Time.timeScale = 0.0625f; // setting to 0 doesn't break anything, but float.Epsilon certainly does
                yield return new WaitForSecondsRealtime(laggy.Item1);
                Time.timeScale = 1;
            }
        }
    }
}
