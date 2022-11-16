using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using System.Diagnostics;

namespace BLChaos.Effects;

internal class RandomTeleport : EffectBase
{
    public RandomTeleport() : base("Teleport to random location") { }
    const float minDist = 25;
    [RangePreference(5, 500, 5)]
    static float distanceVariation = 50;


    public override void HandleNetworkMessage(byte[] location)
    {
        Tele(Utilities.DebyteV3(location));
    }

    public override void OnEffectStart()
    {
        if (isNetworked) return;

#if DEBUG
        int count = 0;
#endif
        Stopwatch sw = Stopwatch.StartNew();
        Vector3 footpos = GlobalVariables.Player_PhysRig.rbFeet.transform.position;
        Vector3 location = footpos;
        while (!IsValid(location))
        {
            location = footpos + Random.onUnitSphere * (minDist + distanceVariation * Random.value);
            if (sw.ElapsedMilliseconds > 1000) break;
#if DEBUG
            count++;
#endif
        }

        if (sw.ElapsedMilliseconds > 1000)
        {
            Utilities.SpawnAd("damn bro youre so ugly i couldn't find anywhere to relocate you. face for radio headass.");
            Log("Failed to find a valid location to teleport the player in 1000ms! WTF?");
            return;
        }

#if DEBUG
        Log($"Found a valid location after {count} tries ({sw.ElapsedMilliseconds}ms).");
#endif
        Tele(location);
        SendNetworkData(location.ToBytes());
    }

    void Tele(Vector3 footPos)
    {
        GlobalVariables.Player_RigManager.Teleport(footPos);
        GlobalVariables.Player_RigManager.physicsRig.ResetHands(SLZ.Handedness.BOTH);
    }

    bool IsValid(Vector3 worlspaceFootPos)
    {
        bool overlapsWithCollider = Physics.CheckCapsule(worlspaceFootPos + Vector3.down, worlspaceFootPos + Vector3.up * 2, 0.75f);
        bool hasGround = Physics.Raycast(worlspaceFootPos, Vector3.down, out RaycastHit hitInfo, 5);
        bool flatGround = hitInfo.normal.y > hitInfo.normal.x * 2 && hitInfo.normal.y > hitInfo.normal.z * 2;
        return hasGround && flatGround && !overlapsWithCollider;
    }
}
