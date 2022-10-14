using ModThatIsNotMod;
using ModThatIsNotMod.Nullables;
using PuppetMasta;
using StressLevelZero.Pool;
using System.Collections;
using System.Linq;
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

        Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");

        yield return new WaitForSeconds(maxWaitTime * Random.value);
        Vector3 spawnPos =
            Player.GetPlayerHead().transform.position +
            new Vector3((Random.value - 0.5f) * 5, 10, (Random.value - 0.5f) * 5);

        
        GameObject c = pool.Spawn(spawnPos, Quaternion.identity, null, true);
        PuppetMaster poppet = c.GetComponentInChildren<PuppetMaster>();
        poppet.StartCoroutine(poppet.DisabledToActive());
    }
}
