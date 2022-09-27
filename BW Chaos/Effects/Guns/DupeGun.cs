using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngineInternal;
using StressLevelZero.Combat;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using ModThatIsNotMod.Nullables;

namespace BWChaos.Effects;

internal class DupeGun : EffectBase
{
    public DupeGun() : base("Dupe Gun", 60) { }
    
    [RangePreference(0, 10, 1)]
    public static int dupeAmount = 1;

    public override void OnEffectStart()
    {
        Hooking.OnPostFireGun += Hooking_OnPostFireGun;
    }
    public override void OnEffectEnd()
    {
        Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
    }

    private void Hooking_OnPostFireGun(Gun gun)
    {
        if (!Physics.Raycast(gun.firePointTransform.position, gun.firePointTransform.forward, out RaycastHit hitInfo, 100)) return;

        Poolee poolee = hitInfo.collider.GetComponentInParent<Poolee>();
        if (poolee == null) return;

        for (int i = 0; i < dupeAmount; i++)
            poolee.pool.Spawn(hitInfo.point, Quaternion.identity, null, true);
    }
}
