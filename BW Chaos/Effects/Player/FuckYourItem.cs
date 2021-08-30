using ModThatIsNotMod;
using StressLevelZero.Interaction;

//todo: this is broken, it crashes the game iirc
namespace BWChaos.Effects
{
    internal class FuckYourItem : EffectBase
    {
        public FuckYourItem() : base("Fuck Your Items") { }

        public override void OnEffectStart()
        {
            Interactable interactable = Player.leftHand.attachedInteractable;
            InteractableHost host = interactable?.GetComponentInParent<InteractableHost>();
            host?.gameObject?.SetActive(false);

            interactable = Player.rightHand.attachedInteractable;
            host = interactable?.GetComponentInParent<InteractableHost>();
            host?.gameObject?.SetActive(false);
        }
    }
}
