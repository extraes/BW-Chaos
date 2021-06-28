using System;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;

namespace BW_Chaos.Effects
{
    internal class Butterfingers : EffectBase
    {
        public Butterfingers() : base("Butterfingers", 60) { }

        public override void OnEffectUpdate()
        {
            // todo: might want a wait in between drops
            Interactable interactable = UnityEngine.Random.Range(0, 2) == 1
                ? Player.leftHand.attachedInteractable
                : Player.rightHand.attachedInteractable;

            if (interactable != null)
            {
                InteractableHost interactableHost = interactable.GetComponentInParent<InteractableHost>();
                interactableHost?.Drop();
            }
        }
    }
}
