using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Combat;
using Steamworks;
using System.Collections.Generic;

namespace BWChaos.Effects
{
    internal class MassEffect : EffectBase
    {
        public MassEffect() : base("Mass Effect", 30) { }
        [EffectPreference("Comma separated list of effect names. If unchanged it will choose 10 effects at random.")] static string effects = "";
        [RangePreference(0, 5, 0.1f)] static float timeBetweenEffets = 1;
        List<EffectBase> effectList = new List<EffectBase>();

        public override void OnEffectStart()
        {
            if (effects == "")
            {
                for (int i = 0; i < 10; i++) effectList.Add(EffectHandler.allEffects.Random().Value);
            }
            else
            {
                string[] effs = effects.Split(',');
                foreach (string eName in effs)
                {
                    if (EffectHandler.allEffects.TryGetValue(eName.Trim().ToLower(), out EffectBase effect))
                        effectList.Add(effect);
                    else
                        Chaos.Warn($"Effect {eName} not found in all effect list, skipping");
                }
            }

            MelonCoroutines.Start(CoRun());
        }

        public IEnumerator CoRun()
        {
            yield return null;
            foreach (EffectBase effect in effectList)
            {
                EffectBase e = (EffectBase)Activator.CreateInstance(effect.GetType());
                e.Run();
                yield return new WaitForSeconds(timeBetweenEffets);
                if (!Active) yield break;
            }
        }
    }
}
