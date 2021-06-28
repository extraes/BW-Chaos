using UnityEngine;
using ModThatIsNotMod;

namespace BW_Chaos.Effects
{
    internal class Accelerate : EffectBase
    {
        public Accelerate() : base("Accelerate", 60) { }
        public override void OnEffectStart()
            => GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration *= 25;
        public override void OnEffectEnd()
            => GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration /= 25;
    }
}
