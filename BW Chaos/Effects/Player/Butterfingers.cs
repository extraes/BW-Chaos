using System;
using System.Collections;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;

namespace BWChaos.Effects
{
    internal class Butterfingers : EffectBase
    {
        public Butterfingers() : base("Butterfingers", 75, EffectTypes.HIDDEN) { }

        public override void OnEffectStart() => MelonCoroutines.Start(CoRun());

        private IEnumerator CoRun()
        {
            yield return null;
            while (Active)
            {
                Interactable interactable = Utilities.GetRandomPlayerHand().attachedInteractable;

                InteractableHost interactableHost = interactable?.GetComponentInParent<InteractableHost>();
                interactableHost?.Drop();

                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(5f,10f));
            }
        }
    }
}
