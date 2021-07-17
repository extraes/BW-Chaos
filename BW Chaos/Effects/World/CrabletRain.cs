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
        public CrabletRain() : base("Crablet Rain", 30) { }

        private List<Poolee> spawnedCrablets = new List<Poolee>();

        public override void OnEffectStart()
        {
            MelonCoroutines.Start(SpawnCrablets());
        }

        private IEnumerator SpawnCrablets()
        {
            // todo: test
            while (Active)
            {
                yield return new WaitForSeconds(10 * UnityEngine.Random.value);
                var spawnPos =
                    Player.GetPlayerHead().transform.position +
                    new Vector3((UnityEngine.Random.value - 0.5f) * 5, 10, (UnityEngine.Random.value - 0.5f) * 5);
                spawnedCrablets.Add(CustomItems.SpawnFromPool("Crablet", spawnPos, Quaternion.identity).GetComponent<Poolee>());
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
