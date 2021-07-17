using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class Centrifuge : EffectBase
    {
        public Centrifuge() : base("Centrifuge", 90) { }

        public override void OnEffectUpdate()
        {
            float r = 1;
            float theta = Time.realtimeSinceStartup % 5 * 360;
            float x = r * (float)Math.Cos(theta * Math.PI / 180);
            float y = r * (float)Math.Sin(theta * Math.PI / 180);
            GlobalVariables.Player_PhysBody.AddVelocityChange(new Vector3(x / 2, 0, y / 2));
        }
    }
}
