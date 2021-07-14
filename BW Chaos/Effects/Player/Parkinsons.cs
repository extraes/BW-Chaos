using UnityEngine;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class Parkinsons : EffectBase
    {
        public Parkinsons() : base("Parkinsons", 90) { }

        public override void OnEffectUpdate()
        {
            Rigidbody handRb = Random.Range(0, 2) == 0 ? Player.rightHand.rb : Player.leftHand.rb;
            handRb.AddRelativeTorque(10 * new Vector3(Random.value - 0.5f, 10 * Random.value - 0.5f, 10 * Random.value - 0.5f));
        }
    }
}
