using StressLevelZero.Pool;
using System;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class GetJumped : EffectBase
    {
        public GetJumped() : base("Get Jumped") { }
        Pool pool;
        const float FPI = (float)Math.PI;

        public override void OnEffectStart()
        {
            pool = pool ?? GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
            if (isNetworked) return;
            if (pool == null) return;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;

            for (int i = 0; i < 8; i++)
            {
                float theta = (i / 8f) * 360;
                float x = Mathf.Cos(theta * FPI / 180);
                float y = Mathf.Sin(theta * FPI / 180);

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
                var spawnedNB = pool.InstantiatePoolee(spawnPos, spawnRot);
                spawnedNB.gameObject.SetActive(true);
                spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
                SendNetworkData(spawnedNB.transform.SerializePosRot());
            }
        }

        public override void HandleNetworkMessage(byte[] data)
        {
            if (pool == null) return;

            var spawnedNB = pool.InstantiatePoolee();
            spawnedNB.transform.DeserializePosRot(data);
            spawnedNB.gameObject.SetActive(true);
            spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
        }
    }
}