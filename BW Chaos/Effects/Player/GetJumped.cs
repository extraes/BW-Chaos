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

        public override void OnEffectStart()
        {
            if (pool == null) pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
            if (pool == null) return;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;

            for (int i = 0; i < 8; i++)
            {
                float theta = (i / 8f) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
                var spawnedNB = pool.InstantiatePoolee(spawnPos, spawnRot);
                spawnedNB.gameObject.SetActive(true);
                spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
                SendNetworkData(string.Join(",",spawnPos.Serialize(4)) + ";" + string.Join(",", playerPos.Serialize(4)));
            }
        }

        public override void HandleNetworkMessage(string data)
        {
            if (pool == null) return;

            string[] datas = data.Split(';');
            Vector3 spawnPos = Utilities.DeserializeV3(datas[0]);
            Vector3 playerPos = Utilities.DeserializeV3(datas[1]);

            var spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
            var spawnedNB = pool.InstantiatePoolee(spawnPos, spawnRot);
            spawnedNB.gameObject.SetActive(true);
            spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
        }
    }
}