using UnityEngine;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class Parkinsons : EffectBase
    {
        public Parkinsons() : base("Parkinsons", 30) { }

        public override void OnEffectUpdate()
        {
            Rigidbody handRb = Utilities.GetRandomPlayerHand().rb;
            handRb.AddRelativeTorque(50 * new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f));
        }
    }
}
