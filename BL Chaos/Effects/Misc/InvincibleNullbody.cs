using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.PuppetMasta;

namespace BLChaos.Effects;

internal class PunchingBagNullbody : EffectBase
{
    public PunchingBagNullbody() : base("Spawn invincible nullbody") { }

    public override async void HandleNetworkMessage(byte[] data)
    {
        Spawnable nbSpawnable = Barcodes.ToSpawnable(JevilBarcode.NULL_BODY);
        if (nbSpawnable == null)
        {
            Chaos.Warn("(networked) Nullbody pool not found! Why?");
            return;
        }

        Poolee nullbody = await nbSpawnable.SpawnAsync(Vector3.zero, Quaternion.identity);
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
        Spawnable nbSpawnable = Barcodes.ToSpawnable(JevilBarcode.NULL_BODY);
        if (nbSpawnable == null)
        {
            Chaos.Warn("Nullbody pool not found! Why?");
            return;
        }

        Poolee nullbody = await nbSpawnable.SpawnAsync(Vector3.zero, Quaternion.identity);
        Utilities.MoveAndFacePlayer(nullbody.gameObject);
        SendNetworkData(nullbody.transform.SerializePosRot());
        nullbody.gameObject.SetActive(true);
        // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
        PuppetMaster pm = nullbody.GetComponentInChildren<PuppetMaster>();
        pm.StartCoroutine(pm.DisabledToActive());
        nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;

    }
}
