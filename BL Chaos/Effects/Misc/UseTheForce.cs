using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using SLZ.Interaction;
using BoneLib;

namespace BLChaos.Effects;

internal class UseTheForce : EffectBase
{
    public UseTheForce() : base("Use The Force", 90) { }
    [RangePreference(0, 25, 1)]
    static float forceMultiplier = 5;
    [RangePreference(0, 10, 1)]
    static float upwardsMultiplier = 1;
    [RangePreference(1, 15, 1)]
    static int updatesPerFrame = 5;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        Transform head = Player.GetPlayerHead().transform;
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();
        yield return null;

        while (Active)
        {
            // FUCK NONALLOC, WE ALLOC IN THIS HOUSE
            Collider[] cols = Physics.OverlapCapsule(head.position, head.position + head.forward * 10, 0.5f);

            yield return null;
            int j = 0;
            for (int i = 0; i < cols.Length; i++)
            {
                Collider col = cols[i];
                Rigidbody arb = col.attachedRigidbody;
                if (arb == null || col.transform.IsChildOfRigManager()) continue;

                float dist = Vector3.Distance(col.transform.position, head.position);
                float forceMult = Mathf.Clamp(1 / dist, 0.1f, 25f);
                Vector3 force = forceMultiplier * forceMult * head.forward;
                force.y *= upwardsMultiplier * Mathf.Sign(force.y);
                arb.AddForce(force, ForceMode.VelocityChange);
                if (j++ % updatesPerFrame == 0) yield return wffu;
            }
#if DEBUG
            Log($"Found {cols.Length} colliders ({j} w/ rb's)");
#endif

            yield return wffu;
        }
    }
}
