using Il2CppSLZ.Marrow.PuppetMasta;

namespace BLChaos.Effects;

internal class CrabletRain : EffectBase
{
    public CrabletRain() : base("Crablet Rain", 30, EffectTypes.DONT_SYNC) { }
    [RangePreference(0.25f, 10, 0.25f)] static float maxWaitTime = 5;

    [AutoCoroutine]
    public IEnumerator SpawnCrablets()
    {
        yield return null;

        Spawnable spawnable = Barcodes.ToSpawnable(JevilBarcode.CRABLET);

        yield return new WaitForSeconds(maxWaitTime * Random.value);
        Vector3 spawnPos = GlobalVariables.Player_PhysRig.m_head.position +
            new Vector3((Random.value - 0.5f) * 5, 10, (Random.value - 0.5f) * 5);
        GameObject spawnedObject = null!;

        
        AssetSpawner.Spawn(spawnable, spawnPos, Quaternion.identity, scale: Utilities.NulledNullable<Vector3>(), groupID: Utilities.NulledNullable<int>(), spawnCallback: new Action<GameObject>((go) => spawnedObject = go));
        while (spawnedObject == null) yield return null;

        PuppetMaster poppet = spawnedObject.GetComponentInChildren<PuppetMaster>();
        poppet.StartCoroutine(poppet.DisabledToActive());
    }
}
