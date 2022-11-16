using Jevil;
using UnityEngine;

namespace BLChaos.Effects;

internal class FlingPlayer : EffectBase
{
    public FlingPlayer() : base("Fling Player") { }
    [RangePreference(0, 5, 0.25f)] static readonly float forceMultiplier = 1f;
    readonly int[] arr = new int[] { -1, 1 };

    public override void OnEffectStart()
    {
        if (isNetworked) return;

        Vector3 vec = new Vector3(9.8f * 1 * arr.Random(), 9.8f * 1.5f, 9.8f * 1 * arr.Random()) * forceMultiplier;
        SendNetworkData(vec.ToBytes()); // idk why 2 decimals
        GlobalVariables.Player_PhysRig.AddVelocityChange(vec);
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        GlobalVariables.Player_PhysRig.AddVelocityChange(Utilities.DebyteV3(data));
    }
}
