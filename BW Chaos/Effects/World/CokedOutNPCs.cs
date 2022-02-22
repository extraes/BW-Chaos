using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using PuppetMasta;
using StressLevelZero.AI;

namespace BWChaos.Effects
{
    internal class CokedOutNPCs : EffectBase
    {
        public CokedOutNPCs() : base("Coked Out NPCs", 180) { }


        public override void OnEffectStart()
        {
            foreach (var brain in Utilities.FindAll<AIBrain>())
            {
                var beh = brain.behaviour;
                if (beh is BehaviourPowerLegs leg)
                {
                    leg.agroedSpeed *= 5f;
                    leg.roamSpeed *= 5f;
                } 
                else if (beh is BehaviourCrablet crab)
                {
                    crab.agroedSpeed *= 5f;
                    crab.roamSpeed *= 5f;
                    crab.jumpCooldown *= 0.001f;
                }
            }
        }

        public override void OnEffectEnd()
        {
            foreach (var brain in Utilities.FindAll<AIBrain>())
            {
                var beh = brain.behaviour;
                if (beh is BehaviourPowerLegs leg)
                {
                    leg.agroedSpeed /= 5f;
                    leg.roamSpeed /= 5f;
                }
                else if (beh is BehaviourCrablet crab)
                {
                    crab.agroedSpeed /= 5f;
                    crab.roamSpeed /= 5f;
                    crab.jumpCooldown /= 0.001f;
                }
            }
        }
    }
}
