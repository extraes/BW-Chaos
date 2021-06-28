using System;
using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class ZeroGravity : EffectBase
    {
        public ZeroGravity() : base("Zero Gravity", 30) { }

        private Vector3 previousGrav;
        private Vector3 zeroGrav = new Vector3(0f, 0.01f, 0f);

        public override void OnEffectStart()
        {
            previousGrav = Physics.gravity;
            Physics.gravity = zeroGrav;
        }

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
