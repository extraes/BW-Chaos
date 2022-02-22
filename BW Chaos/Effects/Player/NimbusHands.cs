using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class NimbusHands : EffectBase
    {
        public NimbusHands() : base("Nimbus Hands", 30) { }
        [RangePreference(0, 10, 0.25f)] static float forceMultiplier = 0.5f;

        public override void OnEffectStart()
        {
            Utilities.MultiplyForces(Player.leftHand.physHand, 100, 5);
            Utilities.MultiplyForces(Player.rightHand.physHand, 100, 5);
        }
        public override void OnEffectEnd()
        {
            Utilities.MultiplyForces(Player.leftHand.physHand);
            Utilities.MultiplyForces(Player.rightHand.physHand);
        }
        public override void OnEffectUpdate()
        {
            // only do it to one hand at a time because framerate is high enough to make it not matter
            var hand = Time.frameCount % 2 == 0 ? Player.leftHand : Player.rightHand;
            var vel = hand.rb.velocity;

            // make sure the velocity is velocity relative to the body
            vel -= GlobalVariables.Player_PhysBody.rbPelvis.velocity;

            vel *= forceMultiplier * Time.deltaTime * 100; // effectively square it to make smaller movements not move as much

            vel.x *= 0.5f;
            vel.z *= 0.5f;

            GlobalVariables.Player_PhysBody.AddVelocityChange(-Vector3.ClampMagnitude(vel, 5 * forceMultiplier));
        }
    }
}
