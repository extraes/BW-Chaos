using BoneLib;
using BoneLib.Nullables;
using Jevil;
using Jevil.Spawning;
using PuppetMasta;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class CrabletRain : EffectBase
{
    public CrabletRain() : base("Crablet Rain", 30, EffectTypes.DONT_SYNC) { }
    [RangePreference(0.25f, 10, 0.25f)] static readonly float maxWaitTime = 5;

    [AutoCoroutine]
    public IEnumerator SpawnCrablets()
    {
        yield return null;

        Spawnable spawnable = Barcodes.ToSpawnable(JevilBarcode.CRABLET);

        yield return new WaitForSeconds(maxWaitTime * Random.value);
        Vector3 spawnPos =
            Player.playerHead.position +
            new Vector3((Random.value - 0.5f) * 5, 10, (Random.value - 0.5f) * 5);
        GameObject spawnedObject = null;


        NullableMethodExtensions.PoolManager_Spawn(spawnable, spawnPos, Quaternion.identity, spawnCallback: new Action<GameObject>((go) => spawnedObject = go));
        while (spawnedObject.INOC()) yield return null;

        PuppetMaster poppet = spawnedObject.GetComponentInChildren<PuppetMaster>();
        poppet.StartCoroutine(poppet.DisabledToActive());
    }
}
