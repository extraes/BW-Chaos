using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class FlingPlayer : EffectBase
    {
        public FlingPlayer() : base("Fling Player") { }
        readonly int[] arr = new int[] { -1, 1 };

        public override void OnEffectStart()
        {
            
            GlobalVariables.Player_PhysBody.AddVelocityChange(
                new Vector3(9.8f * 1 * arr.Random(), 9.8f * 1.5f, 9.8f * 1 * arr.Random()));
        }
    }
}
