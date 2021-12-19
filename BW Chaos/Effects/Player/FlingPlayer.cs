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
            if (isNetworked) return;

            Vector3 vec = new Vector3(9.8f * 1 * arr.Random(), 9.8f * 1.5f, 9.8f * 1 * arr.Random());
            SendNetworkData(vec.Serialize(2).Join()); // idk why 2 decimals
            GlobalVariables.Player_PhysBody.AddVelocityChange(vec);
        }

        public override void HandleNetworkMessage(string data)
        {
            Vector3 vec = Utilities.DeserializeV3(data);
            GlobalVariables.Player_PhysBody.AddVelocityChange(vec);
        }
    }
}
