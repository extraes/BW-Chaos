using System;
using UnityEngine;

namespace BWChaos.Effects;

internal class California : EffectBase
{
    public California() : base("California", 30, EffectTypes.AFFECT_GRAVITY) { }
    [RangePreference(0.25f, 5f, 0.25f)] private static float forceMultiplier = 1f;

    public override void HandleNetworkMessage(byte[] data)
    {
        forceMultiplier = BitConverter.ToSingle(data, 0);
    }

    public override void OnEffectStart()
    {
        SendNetworkData(BitConverter.GetBytes(forceMultiplier));
    }

    public override void OnEffectUpdate()
    {
        //todo: would it make any difference if it was linear instead of a circle? computationally and gameplay-wise i mean
        float theta = (Time.realtimeSinceStartup - StartTime) % 3 * 360;
        //                                      ^ use this so its more "consistent" over entanglement
        float x = Mathf.Cos(theta * Const.FPI / 180);
        float y = Mathf.Sin(theta * Const.FPI / 180);
        float updown = Mathf.Sin(theta * 3 * Const.FPI / 180) - 0.15f;
        Physics.gravity = new Vector3(x * 10, updown * 25, y * 10) * forceMultiplier;

    }

    public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
}
