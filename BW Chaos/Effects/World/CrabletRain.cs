using ModThatIsNotMod;
using StressLevelZero.Pool;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class CrabletRain : EffectBase
    {
        public CrabletRain() : base("Crablet Rain", 30, EffectTypes.DONT_SYNC) { }
        [RangePreference(0.25f, 10, 0.25f)] static float maxWaitTime = 5;

        [AutoCoroutine]
        public IEnumerator SpawnCrablets()
        {
            yield return null;

            Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");

            yield return new WaitForSeconds(maxWaitTime * Random.value);
            var spawnPos =
                Player.GetPlayerHead().transform.position +
                new Vector3((Random.value - 0.5f) * 5, 10, (Random.value - 0.5f) * 5);

            var c = pool.InstantiatePoolee(spawnPos, Quaternion.identity);
            c.gameObject.SetActive(true);
            c.StartCoroutine(c.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
        }
    }
}
