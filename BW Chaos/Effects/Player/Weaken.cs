using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using StressLevelZero.VRMK;

namespace BWChaos.Effects
{
    internal class Weaken : EffectBase
    {
        public Weaken() : base("What 0 Pussy Does to a MF", 15) { }

        public override void OnEffectStart()
        {
            //GlobalVariables.Player_RigManager.physicsRig.EnableBallLoco();
            Utilities.MultiplyForces(Player.leftHand.physHand, (1/5f), (1/5f));
            Utilities.MultiplyForces(Player.rightHand.physHand, (1/5f), (1/5f));
        }

        public override void OnEffectEnd()
        {
            //GlobalVariables.Player_RigManager.physicsRig.DisableBallLoco();
            Utilities.MultiplyForces(Player.leftHand.physHand, 1, 1);
            Utilities.MultiplyForces(Player.rightHand.physHand, 1, 1);
        }
    }
}
