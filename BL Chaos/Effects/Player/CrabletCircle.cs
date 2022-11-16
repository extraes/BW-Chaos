using BoneLib.Nullables;
using Jevil;
using Jevil.Spawning;
using PuppetMasta;
using SLZ.Marrow.Pool;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class CrabletCircle : EffectBase
{
    public CrabletCircle() : base("Crablet Circle") { }

    public override async void OnEffectStart()
    {
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.CRABLET);
        if (isNetworked) return;
        Vector3 playerPos = GlobalVariables.Player_PhysRig.feet.transform.position;

        for (int i = 0; i < 8; i++)
        {
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
            AssetPoolee spawnedNB = await pool.Spawn(spawnPos, spawnRot, null, true).ToTask();
            Utilities.ReMain();
            PuppetMaster poppet = spawnedNB.GetComponentInChildren<PuppetMaster>();
            poppet.StartCoroutine(poppet.DisabledToActive());

            SendNetworkData(spawnedNB.transform.SerializePosRot());
        }
    }

    public override async void HandleNetworkMessage(byte[] data)
    {
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.CRABLET);

        AssetPoolee spawnedNB = await pool.Spawn(Vector3.zero, Quaternion.identity, null, false).ToTask();
        spawnedNB.transform.DeserializePosRot(data);
        spawnedNB.gameObject.SetActive(true);
        PuppetMaster poppet = spawnedNB.GetComponentInChildren<PuppetMaster>();
        poppet.StartCoroutine(poppet.DisabledToActive());
    }
}
