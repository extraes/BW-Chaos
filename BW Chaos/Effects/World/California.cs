using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class California : EffectBase
    {
        public California() : base("California", 30) { }

        private Vector3 previousGrav;

        public override void OnEffectStart() => previousGrav = Physics.gravity;

        public override void OnEffectUpdate()
        {
            float theta = Time.realtimeSinceStartup % 3 * 360;
            float x = (float)(Math.Cos(theta * Math.PI / 180));
            float y = (float)(Math.Sin(theta * Math.PI / 180));
            float updown = (float)(Math.Sin(theta * 3 * Math.PI / 180) - 0.15f);
            Physics.gravity = new Vector3(x * 10, updown * 25, y * 10);
        }

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
