using ModThatIsNotMod.Nullables;
using PuppetMasta;
using StressLevelZero.AI;
using StressLevelZero.Pool;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class PunchingBagNullbody : EffectBase
{
    public PunchingBagNullbody() : base("Spawn invincible nullbody") { }

    public override void HandleNetworkMessage(byte[] data)
    {
        Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
        if (pool == null)
        {
            Chaos.Warn("(networked) Nullbody pool not found! Why?");
            return;
        }

        GameObject nullbody = pool.Spawn(Vector3.zero, Quaternion.identity, null, false);
        PuppetMaster pm = nullbody.GetComponentInChildren<PuppetMaster>();
        Utilities.MoveAndFacePlayer(nullbody.gameObject);
        nullbody.SetActive(true);
        // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
        nullbody.transform.DeserializePosRot(data);
        pm.StartCoroutine(pm.DisabledToActive());
        nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;
    }

    public override void OnEffectStart()
    {
        if (isNetworked) return;
        Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
        if (pool == null)
        {
            Chaos.Warn("Nullbody pool not found! Why?");
            return;
        }

        GameObject nullbody = pool.Spawn(Vector3.zero, Quaternion.identity, null, false);
        Utilities.MoveAndFacePlayer(nullbody);
        SendNetworkData(nullbody.transform.SerializePosRot());
        nullbody.gameObject.SetActive(true);
        // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
        PuppetMaster pm = nullbody.GetComponentInChildren<PuppetMaster>();
        pm.StartCoroutine(pm.DisabledToActive());
        nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;

    }
}
