using UnityEngine;
using ModThatIsNotMod;

namespace BW_Chaos.Effects
{
    internal class Paralyze : EffectBase
    {
        public Paralyze() : base("Paralyze", 30) { }

        private float previousAccel;

        private Vector3 playerPosition;
        private Quaternion playerRotation;

        public override void OnEffectStart()
        {
            previousAccel = GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration;
            GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = float.Epsilon;

            playerPosition = GlobalVariables.Player_PhysBody.transform.position;
            playerRotation = GlobalVariables.Player_PhysBody.transform.rotation;
        }

        public override void OnEffectUpdate()
        {
            // todo: this may break unless we use some custom maps like trickery
            GlobalVariables.Player_PhysBody.transform.position = playerPosition;
            GlobalVariables.Player_PhysBody.transform.rotation = playerRotation;
        }

        public override void OnEffectEnd()
            => GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = previousAccel;
    }
}
