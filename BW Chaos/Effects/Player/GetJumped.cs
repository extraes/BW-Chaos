using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using StressLevelZero.Pool;

namespace BWChaos.Effects
{
    internal class GetJumped : EffectBase
    {
        public GetJumped() : base("Get Jumped") { }

        private List<GameObject> spawnedNulls = new List<GameObject>();
        private GameObject nullPrefab;

        public override void OnEffectStart()
        {
            nullPrefab = PoolManager.GetPool("Nullbody").Prefab;
            MelonCoroutines.Start(SpawnNulls());
        }

        private IEnumerator SpawnNulls()
        {
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            for (int i = 0; i < 7; i++)
            {
                float theta = Time.realtimeSinceStartup % 1 * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                spawnedNulls.Add(GameObject.Instantiate(nullPrefab, spawnPos, Quaternion.identity));
                yield return new WaitForSeconds(2.5f);
            }
        }

        public override void OnEffectEnd()
        {
            foreach (GameObject go in spawnedNulls)
                GameObject.Destroy(go);
            spawnedNulls.Clear();
        }
    }
}
