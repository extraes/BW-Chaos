using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using StressLevelZero.Pool;
using ModThatIsNotMod;
using System.Linq;

namespace BWChaos.Effects
{
    internal class CrabletRain : EffectBase
    {
        public CrabletRain() : base("Crablet Rain", 30) { }

        private List<Poolee> spawnedCrablets = new List<Poolee>();

        public override void OnEffectStart()
        {
            MelonCoroutines.Start(SpawnCrablets());
        }

        private IEnumerator SpawnCrablets()
        {
            yield return null;

            Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");
            
            while (Active)
            {
                yield return new WaitForSeconds(5 * UnityEngine.Random.value);
                var spawnPos =
                    Player.GetPlayerHead().transform.position +
                    new Vector3((UnityEngine.Random.value - 0.5f) * 5, 10, (UnityEngine.Random.value - 0.5f) * 5);

                var c = pool.InstantiatePoolee(spawnPos, Quaternion.identity);
                c.gameObject.SetActive(true);
                spawnedCrablets.Add(c);
            }
        }

        public override void OnEffectEnd()
        {
            foreach (Poolee p in spawnedCrablets)
                p.gameObject.SetActive(false);
            spawnedCrablets.Clear();
        }
    }
}
