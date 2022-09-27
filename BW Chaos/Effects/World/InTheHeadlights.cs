using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BWChaos.Effects;

internal class InTheHeadlights : EffectBase
{
    public InTheHeadlights() : base("In The Headlights", 60) { }
    List<Rigidbody> affectedBodies = new();
    [RangePreference(0, 100, 2)]
    static float dist = 10;

    public override void HandleNetworkMessage(string data)
    {
        GameObject go = GameObject.Find(data);
        Rigidbody rb = go?.GetComponent<Rigidbody>();
        if (rb == null) return;
        if (affectedBodies.Contains(rb)) return;

        rb.isKinematic = true;
    }

    public override void OnEffectEnd()
    {
        foreach (Rigidbody rb in affectedBodies)
        {
            if (rb == null) continue;
            rb.isKinematic = false;
        }
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        WaitForFixedUpdate wffu = new WaitForFixedUpdate();
        Transform head = GlobalVariables.Player_PhysBody.rbHead.transform;

        yield return null;

        // dont feel like dictating things to host n then dealing with peers potentially not seeing it :/
        if (isNetworked) yield break;

        while (Active)
        {
            var cols = Physics.OverlapCapsule(head.position, head.position + (head.forward * dist), 1f);

#if DEBUG
            Log("Found " + cols.Length + " cols");
#endif

            for (int i = 0; i < cols.Count; i++)
            {
                Collider col = cols[i];
                Rigidbody rb = col != null ? col.attachedRigidbody : null;
                if (rb == null || rb.transform.IsChildOfRigManager()) continue;

#if DEBUG
                Log("Kinematicizing " + rb.name);
#endif
                if (!affectedBodies.Contains(rb)) affectedBodies.Add(rb);
                rb.isKinematic = true;

                if (i % 5 == 0) yield return null;
            }
            yield return wffu;
        }
    }
}
