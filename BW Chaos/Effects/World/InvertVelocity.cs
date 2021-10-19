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
            bool stagger = false;
            foreach (var rb in GameObject.FindObjectsOfType<Rigidbody>())
            {
                if (rb.IsSleeping()) continue; // I don't fuck with the lames
                rb.velocity = -rb.velocity;
                if (stagger = !stagger) yield return new WaitForFixedUpdate();
            }
        }
    }
}
