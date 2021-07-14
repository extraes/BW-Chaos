using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using StressLevelZero.Pool;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class CrabletRain : EffectBase
    {
        public CrabletRain() : base("Crablet Rain", 120) { }

        private List<GameObject> spawnedCrablets = new List<GameObject>();
        private GameObject crabletPrefab;

        public override void OnEffectStart()
        {
            crabletPrefab = PoolManager.GetPool("Crablet").Prefab;
            MelonCoroutines.Start(SpawnCrablets());
        }

        private IEnumerator SpawnCrablets()
        {
            // todo: nullrefs
            while (Active)
            {
                yield return new WaitForSeconds(10 * UnityEngine.Random.value);
                var spawnPos =
                    Player.GetPlayerHead().transform.position +
                    new Vector3((UnityEngine.Random.value - 0.5f) * 5, 10, (UnityEngine.Random.value - 0.5f) * 5);
                spawnedCrablets.Add(GameObject.Instantiate(crabletPrefab, spawnPos, Quaternion.identity));
            }
        }

        public override void OnEffectEnd()
        {
            foreach (GameObject go in spawnedCrablets)
                GameObject.Destroy(go);
            spawnedCrablets.Clear();
        }
    }
}
