using UnityEngine;

namespace BWChaos.Effects;

internal class Parkinsons : EffectBase
{
    public Parkinsons() : base("Parkinsons", 30) { }
    [RangePreference(0, 25, 0.125f)] static readonly float forceMultiplier = 1;

    public override void OnEffectUpdate()
    {
        Rigidbody handRb = Utilities.GetRandomPlayerHand().rb;
        handRb.AddRelativeTorque(forceMultiplier * 75 * new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f));
    }
}
