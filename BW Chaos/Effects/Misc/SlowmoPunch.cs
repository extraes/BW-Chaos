using HarmonyLib;
using MelonLoader;
using StressLevelZero.SFX;
using System;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects
{

    internal class SlowmoPunch : EffectBase
    {
        public SlowmoPunch() : base("SlowMo Punch", 60) { }
        [RangePreference(0, 5f, 0.01f)] static float returnTime = 1f;
        float YieldTime => returnTime / 20;

        public override void OnEffectStart() => OnPunch += RunSlowmo;

        public override void OnEffectEnd() => OnPunch -= RunSlowmo;

        public void RunSlowmo() => MelonCoroutines.Start(Slowmo_OnPunch());

        public static event Action OnPunch;
        [HarmonyPatch(typeof(HandSFX), "PunchAttack")]
        public static class PunchPatch
        {
            // Hopefully prevent this from throwing nullrefs
            public static void Postfix() => OnPunch?.Invoke();
        }

        private IEnumerator Slowmo_OnPunch()
        {
            Time.timeScale = 0.05f;
            
            while (Time.timeScale < 1f && !Active)
            {
                Time.timeScale += 0.05f;
                yield return new WaitForSecondsRealtime(YieldTime);
            }
            Time.timeScale = 1;
        }
    }
}
