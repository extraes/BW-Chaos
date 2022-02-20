using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using StressLevelZero.VRMK;

namespace BWChaos.Effects
{
    internal class Strengthen : EffectBase
    {
        public Strengthen() : base("Live Simulation of ME (Chad)", 60) { }

        public override void OnEffectStart()
        {
            Utilities.MultiplyForces(Player.leftHand.physHand);
            Utilities.MultiplyForces(Player.rightHand.physHand);
        }

        public override void OnEffectEnd()
        {
            Utilities.MultiplyForces(Player.leftHand.physHand, 1, 1);
            Utilities.MultiplyForces(Player.rightHand.physHand, 1, 1);
        }
    }
}
