using BoneLib.Nullables;
using Jevil;
using Jevil.Spawning;
using PuppetMasta;
using SLZ.AI;
using SLZ.Marrow.Pool;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class PunchingBagNullbody : EffectBase
{
    public PunchingBagNullbody() : base("Spawn invincible nullbody") { }

    public override async void HandleNetworkMessage(byte[] data)
    {
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.NULL_BODY);
        if (pool == null)
        {
            Chaos.Warn("(networked) Nullbody pool not found! Why?");
            return;
        }

        AssetPoolee nullbody = await pool.Spawn(Vector3.zero, Quaternion.identity, null, false).ToTask();
        PuppetMaster pm = nullbody.GetComponentInChildren<PuppetMaster>();
        Utilities.MoveAndFacePlayer(nullbody.gameObject);
        nullbody.gameObject.SetActive(true);
        // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
        nullbody.transform.DeserializePosRot(data);
        pm.StartCoroutine(pm.DisabledToActive());
        nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;
    }

    public override async void OnEffectStart()
    {
        if (isNetworked) return;
        AssetPool pool = Barcodes.ToAssetPool(JevilBarcode.NULL_BODY);
        if (pool == null)
        {
            Chaos.Warn("Nullbody pool not found! Why?");
            return;
        }

        AssetPoolee nullbody = await pool.Spawn(Vector3.zero, Quaternion.identity, null, false).ToTask();
        Utilities.MoveAndFacePlayer(nullbody.gameObject);
        SendNetworkData(nullbody.transform.SerializePosRot());
        nullbody.gameObject.SetActive(true);
        // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
        PuppetMaster pm = nullbody.GetComponentInChildren<PuppetMaster>();
        pm.StartCoroutine(pm.DisabledToActive());
        nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;

    }
}
