using ModThatIsNotMod.Nullables;
using PuppetMasta;
using StressLevelZero.Pool;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects;

internal class GetJumped : EffectBase
{
    public GetJumped() : base("Get Jumped") { }
    Pool pool;

    public override void OnEffectStart()
    {
        pool = pool != null ? pool : GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
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
            GameObject spawnedNB = pool.Spawn(spawnPos, spawnRot, null, true);
            PuppetMaster pm = spawnedNB.GetComponentInChildren<PuppetMaster>();
            spawnedNB.gameObject.SetActive(true);
            pm.StartCoroutine(pm.DisabledToActive());
            SendNetworkData(spawnedNB.transform.SerializePosRot());
        }
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        if (pool == null) return;

        GameObject spawnedNB = pool.Spawn(Vector3.zero, Quaternion.identity, null, false);
        PuppetMaster pm = spawnedNB.GetComponentInChildren<PuppetMaster>();
        spawnedNB.transform.DeserializePosRot(data);
        spawnedNB.SetActive(true);
        pm.StartCoroutine(pm.DisabledToActive());
    }
}
