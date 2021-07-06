using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class FlingPlayer : EffectBase
    {
        public FlingPlayer() : base("Fling Player") { }

        public override void OnEffectStart()
        {
            int[] arr = new int[] { -1, 1 };
            GlobalVariables.Player_PhysBody.AddVelocityChange(
                new Vector3(9.8f * 3 * arr[Random.Range(0, 2)], 9.8f * 6, 9.8f * 3 * arr[Random.Range(0, 2)]));
        }
    }
}
