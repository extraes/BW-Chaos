using MelonLoader;
using Jevil;
using SLZ.Marrow.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using BoneLib;
using SLZ.Props.Weapons;
using SLZ.Combat;
using Jevil.Patching;
using HarmonyLib;

namespace BLChaos.Effects;

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Awake))]
internal class UpNAtomize : EffectBase
{
    public UpNAtomize() : base("Up-N-Atomize", 90, EffectTypes.LAGGY) { }
    [RangePreference(0, 10, 0.25f)] static readonly float radius = 2f;
    [RangePreference(0, 50, 0.5f)] static readonly float forceMultiplier = 1f;
    [RangePreference(1, 10, 1)] static readonly int rbsPerFrame = 4;

    static Action<Collider, Vector3, Vector3> onBulletHit;

    public override void OnEffectStart()
    {
        onBulletHit += OnBulletHit;
    }

    private void OnBulletHit(Collider col, Vector3 pos, Vector3 normal)
    {
#if DEBUG
        Chaos.Log("Bullet collided with " + col.name);
        Chaos.Log("Vector3 argument 2 = " + pos.ToString());
        Chaos.Log("Vector3 argument 3 = " + normal.ToString());
#endif

        Collider[] cols = Physics.OverlapSphere(pos, radius * 1.25f);
        MelonCoroutines.Start(ApplyForces(cols.Take(cols.Length / 2), pos));
        MelonCoroutines.Start(ApplyForces(cols.Skip(cols.Length / 2), pos));
    }

    private IEnumerator ApplyForces(IEnumerable<Collider> cols, Vector3 origin)
    {
        yield return null;
        int counter = 0;
        foreach (Collider col in cols)
        {
            if (col.INOC() || col.attachedRigidbody == null || col.attachedRigidbody.isKinematic) continue;
            Rigidbody rb = col.attachedRigidbody;
            rb.AddExplosionForce(rb.mass * forceMultiplier * 25, origin, radius, 2);
            if (counter++ % rbsPerFrame == 0) yield return null;
        }
    }

    [HarmonyPostfix]
    private static void Projectile_Awake(Projectile __instance)
    {
        __instance.onCollision.AddListener(new Action<Collider, Vector3, Vector3>(OnBulletHitImpl));
    }

    private static void OnBulletHitImpl(Collider col, Vector3 pos, Vector3 normal)
    => onBulletHit?.Invoke(col, pos, normal);
}
