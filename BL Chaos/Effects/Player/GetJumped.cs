
using BoneLib.Nullables;
using Jevil;
using Jevil.Spawning;
using PuppetMasta;
using SLZ.Marrow.Pool;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class GetJumped : EffectBase
{
    public GetJumped() : base("Get Jumped") { }

    public override async void OnEffectStart()
    {
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.NULL_BODY);
        if (isNetworked) return;
        if (pool == null) return;
        Vector3 playerPos = GlobalVariables.Player_PhysRig.feet.transform.position;

        for (int i = 0; i < 8; i++)
        {
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
            AssetPoolee spawnedNB = await pool.Spawn(spawnPos, spawnRot, null, true).ToTask();
            PuppetMaster pm = spawnedNB.GetComponentInChildren<PuppetMaster>();
            spawnedNB.gameObject.SetActive(true);
            pm.StartCoroutine(pm.DisabledToActive());
            SendNetworkData(spawnedNB.transform.SerializePosRot());
        }
    }

    public override async void HandleNetworkMessage(byte[] data)
    {
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.NULL_BODY);

        (Vector3 pos, Quaternion rot) = Utilities.DebytePosRot(data);
        AssetPoolee spawnedNB = await pool.Spawn(pos, rot, null, true).ToTask();
        PuppetMaster pm = spawnedNB.GetComponentInChildren<PuppetMaster>();
        pm.StartCoroutine(pm.DisabledToActive());
    }
}
