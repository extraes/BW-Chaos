﻿using System;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class TripleThreat : EffectBase
    {
        public TripleThreat() : base("Triple threat", 15, EffectTypes.META) { }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            if (isNetworked) yield break;

            while (Active)
            { 
                EffectBase og = EffectHandler.AllEffects.Values.Random();
                EffectBase newie = (EffectBase)Activator.CreateInstance(og.GetType());
                newie.Run();

                yield return new WaitForSecondsRealtime(5f);
            }
        }
    }
}