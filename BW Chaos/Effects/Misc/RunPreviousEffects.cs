using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class RunPreviousEffects : EffectBase
    {
        public RunPreviousEffects() : base("Run Previous Effects", 35, EffectTypes.META) { }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            foreach (var eName in GlobalVariables.PreviousEffects)
            {
                if (!Active) yield break;
                if (eName == Name) continue; // avoid infinite loop

                if (EffectHandler.AllEffects.TryGetValue(eName, out EffectBase original))
                {
                    EffectBase effect = (EffectBase)Activator.CreateInstance(original.GetType());
                    effect.Run();
                }
                else Utilities.SpawnAd("I see you've changed your effects since " + eName + " was ran. ok...");

                yield return new WaitForSecondsRealtime(5f);
            }
            ForceEnd();
        }
    }
}
