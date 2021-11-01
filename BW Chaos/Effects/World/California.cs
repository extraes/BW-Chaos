using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class California : EffectBase
    {
        public California() : base("California", 30, EffectTypes.AFFECT_GRAVITY) { }

        private const float FPI = (float)Math.PI;
        private Vector3 previousGrav;

        public override void OnEffectStart() => previousGrav = Physics.gravity;

        public override void OnEffectUpdate()
        {
            //todo: would it make any difference if it was linear instead of a circle? computationally and gameplay-wise i mean
            float theta = (Time.realtimeSinceStartup - StartTime) % 3 * 360;
            //                                      ^ use this so its more "consistent" over entanglement
            float x = Mathf.Cos(theta * FPI / 180);
            float y = Mathf.Sin(theta * FPI / 180);
            float updown = Mathf.Sin(theta * 3 * FPI / 180) - 0.15f;
            Physics.gravity = new Vector3(x * 10, updown * 25, y * 10);
            
        }

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
