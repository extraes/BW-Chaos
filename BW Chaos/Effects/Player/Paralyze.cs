using UnityEngine;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class Paralyze : EffectBase
    {
        public Paralyze() : base("Paralyze", 20) { }

        public override void OnEffectStart()
            => GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 0;

        public override void OnEffectEnd() 
            => GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 2;
    }
}
