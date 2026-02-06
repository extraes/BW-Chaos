#if !NOBONELIB
#endif

namespace BLChaos.Effects;

internal class DupeGun : EffectBase
{
    public DupeGun() : base("Dupe Gun", 60) { }
    
    [RangePreference(0, 10, 1)]
    public static int dupeAmount = 1;

    public override void OnEffectStart()
    {
#if NOBONELIB
        throw new NotImplementedException("This effect requires BoneLib to function");
#else
        Hooking.OnPostFireGun += Hooking_OnPostFireGun;
#endif
    }
    public override void OnEffectEnd()
    {
#if !NOBONELIB
        Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
#endif
    }

    private void Hooking_OnPostFireGun(Gun gun)
    {
        if (!Physics.Raycast(gun.firePointTransform.position, gun.firePointTransform.forward, out RaycastHit hitInfo, 100)) return;

        Poolee poolee = hitInfo.collider.GetComponentInParent<Poolee>();
        if (poolee == null) return;

        for (int i = 0; i < dupeAmount; i++)
            poolee.SpawnableCrate.Spawn(hitInfo.point, Quaternion.identity);
    }

#if DEBUG
    internal override async Task<TestResult> Test()
    {
        Spawnable gymBlock = Barcodes.ToSpawnable(JevilBarcode.GYM_BLOCK_B);
        Pool spawnablePool = AssetSpawner._instance._barcodeToPool[gymBlock.crateRef.Barcode];
        int preSpawned = spawnablePool._spawned.Count;
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

        postSpawned = spawnablePool._spawned.Count;
        Log("Postspawn count: " + postSpawned);
        return Res(preSpawned < postSpawned);
    }
#endif
}
