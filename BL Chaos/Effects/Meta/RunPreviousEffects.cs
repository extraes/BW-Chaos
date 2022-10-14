using System;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class RunPreviousEffects : EffectBase
{
    public RunPreviousEffects() : base("Run Previous Effects", 35, EffectTypes.META) { }
    private readonly string[] pEffects = new string[7];

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        // copy to temp array to avoid concurrent modification
        GlobalVariables.PreviousEffects.CopyTo(pEffects);
        foreach (string eName in pEffects)
        {
            if (!Active) yield break;
            if (eName == Name) continue; // avoid infinite loop

            if (EffectHandler.allEffects.TryGetValue(eName, out EffectBase original))
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
