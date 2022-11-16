using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class RandomGravity : EffectBase
{
    public RandomGravity() : base("Random Gravity", 60, EffectTypes.LAGGY | EffectTypes.AFFECT_GRAVITY) { }
    [RangePreference(0, 50, 1)]
    static float minStrength = 5;
    [RangePreference(0, 50, 1)]
    static float strengthVariation = 25;
    [RangePreference(0, 50, 1)]
    static float minWaitTime = 5;
    [RangePreference(0, 50, 1)]
    static float waitTimeVariation = 15;


    public override void HandleNetworkMessage(byte[] data)
    {
        Physics.gravity = Utilities.DebyteV3(data);
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        if (isNetworked) yield break;

        while(Active)
        {
            Physics.gravity = Random.onUnitSphere * (minStrength + Random.value * strengthVariation);
            SendNetworkData(Physics.gravity.ToBytes());
            yield return new WaitForSeconds(minWaitTime + waitTimeVariation * Random.value);
        }
    }
}
