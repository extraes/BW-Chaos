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
        public Butterfingers() : base("Butterfingers", 75) { }

        public override void OnEffectStart() => MelonCoroutines.Start(CoRun());

        private IEnumerator CoRun()
        {
            while (Active)
            {
                Interactable interactable = UnityEngine.Random.Range(0, 2) == 1
                ? Player.leftHand.attachedInteractable
                : Player.rightHand.attachedInteractable;

                InteractableHost interactableHost = interactable?.GetComponentInParent<InteractableHost>();
                interactableHost?.Drop();

                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(5f,10f));
            }
        }
    }
}
