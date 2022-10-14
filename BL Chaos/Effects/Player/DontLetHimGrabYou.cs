using ModThatIsNotMod;
using StressLevelZero.Interaction;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class DontLetHimGrabYou : EffectBase
{
    public DontLetHimGrabYou() : base("DONT LET HIM GRAB YOU", 60) { }
    [RangePreference(1, 10, 0.5f)] static readonly float minGrabTime = 3;
    [RangePreference(2, 20, 0.5f)] static readonly float minWaitTime = 5;
    [RangePreference(1, 10, 0.25f)] static readonly float forceMultiplier = 1;

    Hand hand = Player.leftHand;
    Vector3 dir = Random.onUnitSphere;
    bool enabled = false;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        float maxGrabTime = minGrabTime + Random.Range(0, 6);
        float maxWaitTime = minWaitTime + Random.Range(0, 6);
        while (Active)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            hand = Random.value > 0.5f ? Player.leftHand : Player.rightHand;
            dir = (Random.onUnitSphere + Vector3.up).normalized;
            enabled = true;
            yield return new WaitForSeconds(Random.Range(minGrabTime, maxGrabTime));
            enabled = false;
        }
    }

    public override void OnEffectUpdate()
    {
        if (enabled)
        {
            hand.rb.AddForce(8 * forceMultiplier * dir, ForceMode.VelocityChange);
        }
    }
}
