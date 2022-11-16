using BoneLib;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class ExplosiveGuns : EffectBase
{
    public ExplosiveGuns() : base("Explosive Guns", 60) { }
    [RangePreference(0.125f, 5, 0.125f)] static readonly float forceMultiplier = 1;

    public override void OnEffectStart() => Hooking.OnPostFireGun += Hooking_OnPostFireGun;

    public override void OnEffectEnd() => Hooking.OnPostFireGun -= Hooking_OnPostFireGun;

    private void Hooking_OnPostFireGun(SLZ.Props.Weapons.Gun obj)
    {
        Vector3 origin = obj.firePointTransform.position;

        Collider[] cols = Physics.OverlapSphere(origin, 2);
        string rmn = GlobalVariables.Player_RigManager.name;
        IEnumerable<Rigidbody> rbs = cols.Where(c => !c.transform.InHierarchyOf(rmn)).Select(c => c.attachedRigidbody).Distinct();

        MelonCoroutines.Start(ApplyForces(rbs.Skip(rbs.Count() / 2), origin));
        MelonCoroutines.Start(ApplyForces(rbs.Take(rbs.Count() / 2), origin));
    }

    // yoinking code from fartwithreverb lol
    private IEnumerator ApplyForces(IEnumerable<Rigidbody> rbs, Vector3 origin)
    {
        yield return null;
        bool stagger = false;
        foreach (Rigidbody rb in rbs)
        {
            if (rb == null) continue;
            Vector3 p = rb.transform.position; //our current position
            Vector3 v = rb.velocity; //our current velocity
            Vector3 force = rb.mass * (origin - p - v * Time.deltaTime) / Time.deltaTime;

            rb.AddForce(-Vector3.ClampMagnitude(force * forceMultiplier, 500 * rb.mass));

            if (stagger = !stagger) yield return new WaitForFixedUpdate();
        }
    }
}
