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
            // todo: force may be too much, explosion position may not work properly
            foreach (Rigidbody body in GameObject.FindObjectsOfType<Rigidbody>())
                body.AddExplosionForce(200f, body.transform.position, 5f);
        }
    }
}
