using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class Fling : EffectBase
    {
        public Fling() : base("Fling Everything") { }

        public override void OnEffectStart()
        {
            // todo: test this
            foreach (Rigidbody body in GameObject.FindObjectsOfType<Rigidbody>())
                body.AddExplosionForce(1000f, body.transform.position, 10f);
        }
    }
}
