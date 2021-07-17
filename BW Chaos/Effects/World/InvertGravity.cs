using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class InvertGravity : EffectBase
    {
        public InvertGravity() : base("Invert Gravity", 45) { }

        private Vector3 previousGrav;
        private Vector3 invertedGrav = new Vector3(0f, 0.01f, 0f);

        public override void OnEffectStart()
        {
            previousGrav = Physics.gravity;
            invertedGrav = previousGrav;
            invertedGrav.y = Mathf.Abs(invertedGrav.y);
            Physics.gravity = invertedGrav;
        }

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
