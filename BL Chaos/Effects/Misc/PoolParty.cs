namespace BLChaos.Effects;

internal class PoolParty : EffectBase
{
    public PoolParty() : base("Pool Party", 60) { }
    static IEnumerable<Pool> pools;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;

        if (pools == null || pools.FirstOrDefault() == null) pools = Instances.AllPools;

        if (isNetworked) yield break;
        while (Active)
        {
            Pool pool = pools.Random();
            if (pool._crate.Barcode.ID.Contains("SLZ.BONELAB.Core.Spawnable.RigManager"))
                continue;
            Log("Spawning pool: " + pool._crate.Barcode.ID);
            pool.Spawn(GlobalVariables.inFrontOfPlayer, GlobalVariables.lookingAtPlayer, Jevil.Utilities.NulledNullable<Vector3>());
            SendNetworkData(Utilities.SerializeInFrontFacingPlayer(), Encoding.ASCII.GetBytes(pool._crate.Barcode.ID));
            yield return new WaitForSeconds(5);
        }
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        string poolBarcode = Encoding.ASCII.GetString(data[1]);

        Spawnable spawnable = Barcodes.ToSpawnable(poolBarcode);
        if (spawnable == null)
        {
            Chaos.Warn("Pool not found in client - ID: " + poolBarcode);
            return;
        }

        (Vector3 pos, Quaternion rot) = Utilities.DebytePosRot(data[0]);
        spawnable.Spawn(pos, rot, true);
    }
}
