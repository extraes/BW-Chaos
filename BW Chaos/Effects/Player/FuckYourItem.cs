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
            Player.GetObjectInHand(Player.leftHand)?.SetActive(false);
            Player.GetObjectInHand(Player.rightHand)?.SetActive(false);
        }
    }
}
