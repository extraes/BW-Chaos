using BoneLib;
using BoneLib.Nullables;
using Jevil;
using Jevil.Spawning;
using PuppetMasta;
using SLZ.Marrow.Pool;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BLChaos.Effects;

internal class CrabletRain : EffectBase
{
    public CrabletRain() : base("Crablet Rain", 30, EffectTypes.DONT_SYNC) { }
    [RangePreference(0.25f, 10, 0.25f)] static readonly float maxWaitTime = 5;

    [AutoCoroutine]
    public IEnumerator SpawnCrablets()
    {
        yield return null;

        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.CRABLET);

        yield return new WaitForSeconds(maxWaitTime * Random.value);
        Vector3 spawnPos =
            Player.GetPlayerHead().transform.position +
            new Vector3((Random.value - 0.5f) * 5, 10, (Random.value - 0.5f) * 5);

        
        Task<AssetPoolee> task = pool.Spawn(spawnPos, Quaternion.identity, null, true).ToTask();
        while (!task.IsCompleted) yield return null;

        PuppetMaster poppet = task.Result.GetComponentInChildren<PuppetMaster>();
        poppet.StartCoroutine(poppet.DisabledToActive());
    }
}
