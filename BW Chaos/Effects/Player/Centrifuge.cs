using System;
using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class Centrifuge : EffectBase
    {
        public Centrifuge() : base("Centrifuge", 30) { }

        public override void OnEffectUpdate()
        {
            // todo: i feel like this could just be done by constantly adding to the player rotation
            float r = 1;
            float theta = Time.realtimeSinceStartup % 5 * 360;
            float x = r * (float)Math.Cos(theta * Math.PI / 180);
            float y = r * (float)Math.Sin(theta * Math.PI / 180);
            GlobalVariables.Player_PhysBody.AddVelocityChange(new Vector3(x / 2, 0, y / 2));
        }
    }
}
