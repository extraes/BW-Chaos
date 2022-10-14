using StressLevelZero.Interaction;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class Butterfingers : EffectBase
{
    public Butterfingers() : base("Butterfingers", 75, EffectTypes.HIDDEN) { }
    [RangePreference(1, 10, 1)] static readonly float minWaitTime = 5;
    [RangePreference(10, 20, 1)] static readonly float maxWaitTime = 10f;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        while (Active)
        {
            Interactable interactable = Utilities.GetRandomPlayerHand().attachedInteractable;

            InteractableHost interactableHost = interactable?.GetComponentInParent<InteractableHost>();
            interactableHost?.Drop();

            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(minWaitTime, maxWaitTime));
        }
    }
}
