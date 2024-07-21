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
using System.Threading.Tasks;
using Jevil.Spawning;
using SLZ.Marrow.Data;
using SLZ.Marrow.Warehouse;
using Cysharp.Threading.Tasks;

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

#if DEBUG
    internal override async Task<TestResult> Test()
    {
        Spawnable gymBlock = Barcodes.ToSpawnable(JevilBarcode.GYM_BLOCK_B);
        AssetPool spawnablePool = AssetSpawner._instance._barcodeToPool[gymBlock.crateRef.Barcode];
        int preSpawned = spawnablePool.spawned.Count;
        int postSpawned;
        Log("Prespawn count: " + preSpawned);

        GameObject gunTmpGo = new GameObject("testgun");
        Gun gunTmp = gunTmpGo.AddComponent<Gun>();
        Log("Created gun");
        Vector3 spawnPoint = gunTmpGo.transform.position + gunTmpGo.transform.forward;
        gunTmp.firePointTransform = gunTmpGo.transform;
        Log("Set values. Spawning item at " + spawnPoint.ToString());

        await gymBlock.SpawnAsync(spawnPoint, Quaternion.identity);

        try
        {
            gunTmp.Fire(); // this will error cuz fields havent been set
        }
        catch
        {
            Log("Test errored on fire, expectedly");
        }

        postSpawned = spawnablePool.spawned.Count;
        Log("Postspawn count: " + postSpawned);
        return Res(preSpawned < postSpawned);
    }
#endif
}
