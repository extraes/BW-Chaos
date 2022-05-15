using StressLevelZero.Pool;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects;

internal class CrabletCircle : EffectBase
{
    public CrabletCircle() : base("Crablet Circle") { }
    static Pool pool;

    public override void OnEffectStart()
    {
        pool = pool != null ? pool : GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");
        if (isNetworked) return;
        if (pool == null) return;
        Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;

        for (int i = 0; i < 8; i++)
        {
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
            Poolee spawnedNB = pool.InstantiatePoolee(spawnPos, spawnRot);
            spawnedNB.gameObject.SetActive(true);
            spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());

            SendNetworkData(spawnedNB.transform.SerializePosRot());
        }
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        if (pool == null) return;

        Poolee spawnedNB = pool.InstantiatePoolee();
        spawnedNB.transform.DeserializePosRot(data);
        spawnedNB.gameObject.SetActive(true);
        spawnedNB.StartCoroutine(spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
    }
}
