using System;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class FuckYourItem : EffectBase
    {
        public FuckYourItem() : base("Fuck Your Items") { }

        public override void OnEffectStart()
        {
            Interactable interactable = Player.leftHand.attachedInteractable;
            InteractableHost host = interactable?.GetComponentInParent<InteractableHost>();
            host?.Drop();
            host?.gameObject?.SetActive(false);

            interactable = Player.rightHand.attachedInteractable;
            host = interactable?.GetComponentInParent<InteractableHost>();
            host?.Drop();
            host?.gameObject?.SetActive(false);
        }
    }
}
