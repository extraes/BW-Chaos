using System;
using UnityEngine;
using MelonLoader;
using System.Collections;

namespace BWChaos.Effects
{
    internal class InvertVelocity : EffectBase
    {
        public InvertVelocity() : base("Invert velocity of everything") { }

        public override void OnEffectStart()
        {
            MelonCoroutines.Start(CoRun());
        }

        private IEnumerator CoRun ()
        {
            foreach (var rb in GameObject.FindObjectsOfType<Rigidbody>())
            {
                if (rb.IsSleeping()) continue; // I don't fuck with the lames
                // When subtracting 2x a number, you invert the number (e.g. 5 - (5*2) = -5)
                rb.AddForce(-2 * rb.velocity, ForceMode.VelocityChange);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
