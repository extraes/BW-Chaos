using System;
using UnityEngine;
using MelonLoader;
using BoneLib;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngineInternal;
using SLZ.Combat;
using SLZ.Props.Weapons;
using SLZ.Marrow.Pool;
using Jevil;

namespace BLChaos.Effects;

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

        AssetPoolee poolee = hitInfo.collider.GetComponentInParent<AssetPoolee>();
        if (poolee == null) return;

        for (int i = 0; i < dupeAmount; i++)
            poolee.spawnableCrate.Spawn(hitInfo.point, Quaternion.identity);
    }
}
