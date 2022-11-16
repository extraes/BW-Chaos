using PuppetMasta;
using SLZ.AI;

namespace BLChaos.Effects;

internal class CokedOutNPCs : EffectBase
{
    public CokedOutNPCs() : base("Coked Out NPCs", 180) { }


    public override void OnEffectStart()
    {
        foreach (AIBrain brain in Utilities.FindAll<AIBrain>())
        {
            BehaviourBaseNav beh = brain.behaviour;
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
        foreach (AIBrain brain in Utilities.FindAll<AIBrain>())
        {
            BehaviourBaseNav beh = brain.behaviour;
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
