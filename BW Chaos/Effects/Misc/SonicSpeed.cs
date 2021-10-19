using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class SonicSpeed : EffectBase
    {
        public SonicSpeed() : base("SANIC SPEED", 60) { }

        public override void OnEffectStart()
        {
            // 10x because who doesnt need a little whiplash in life?
            GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = 70;
            GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 20;
        }

        public override void OnEffectEnd()
        {
            // return base values
            GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = 7;
            GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 2;
        }
    }
}
