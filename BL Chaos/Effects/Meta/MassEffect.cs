using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLChaos.Effects;

internal class MassEffect : EffectBase
{
    public MassEffect() : base("Mass Effect", 30) { }
    [EffectPreference("Comma separated list of effect names. If unchanged it will choose 10 effects at random.")] static readonly string effects = "";
    [RangePreference(0, 5, 0.1f)] static readonly float timeBetweenEffets = 1;
    readonly List<EffectBase> effectList = new List<EffectBase>();

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
                string cleanedName = eName.Trim();
                if (cleanedName == Name)
                {
                    Chaos.Warn($"I see that attempt at recursion! Ignoring effect in list named {cleanedName}!");
                    continue;
                }

                if (EffectHandler.allEffects.TryGetValue(cleanedName, out EffectBase effect))
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
#if DEBUG
        Log($"MassEffect has {effectList.Count} effects. Here's what they are:");
        foreach (EffectBase _e in effectList) Log($"Type={_e.GetType().Name}; Name='{_e.Name}'");
#endif

        foreach (EffectBase effect in effectList)
        {
            EffectBase e = (EffectBase)Activator.CreateInstance(effect.GetType());
            e.Run();
            yield return new WaitForSeconds(timeBetweenEffets);
            if (!Active) yield break;
        }
    }
}
