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
        public SlowmoPunch() : base("SlowMo Punch") { }

        //todo: this is probably bad lole -extraes
        //not the patch, that's fine, but my implementation/adulteration of it. (cause ive 
        public override void OnEffectStart()
        {
            OnPunch += RunSlowmo;
        }
        public override void OnEffectEnd()
        {
            OnPunch -= RunSlowmo;
        }

        public void RunSlowmo(Collision arg1, float arg2, float arg3) => MelonCoroutines.Start(Slowmo_OnPunch(arg1, arg2, arg3));

        public static event Action<Collision, float, float> OnPunch;
        [HarmonyPatch(typeof(HandSFX), "PunchAttack")]
        public static class PunchPatch
        {
            public static void Prefix(Collision c, float impulse, float relVelSqr) => OnPunch(c, impulse, relVelSqr);
        }

        private IEnumerator Slowmo_OnPunch(Collision arg1, float arg2, float arg3)
        {
            Time.timeScale = 0.05f;
            while (Time.timeScale < 1f && !Active)
            {
                Time.timeScale += 0.05f;
                yield return new WaitForSecondsRealtime(0.05f);
            }
            Time.timeScale = 1;
        }
    }
}
