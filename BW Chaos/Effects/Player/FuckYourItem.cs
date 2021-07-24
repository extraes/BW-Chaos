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
            // todo: do we actually need to drop before disabling?
            //todo: after investigating, we don't.   -extraes
            Interactable interactable = Player.leftHand.attachedInteractable;
            InteractableHost host = interactable?.GetComponentInParent<InteractableHost>();
            //host?.Drop();
            host?.gameObject?.SetActive(false);

            interactable = Player.rightHand.attachedInteractable;
            host = interactable?.GetComponentInParent<InteractableHost>();
            //host?.Drop();
            host?.gameObject?.SetActive(false);
        }
    }
}
