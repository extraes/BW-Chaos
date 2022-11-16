
using BoneLib.Nullables;
using Cysharp.Threading.Tasks;
using Jevil;
using Jevil.Spawning;
using SLZ.Marrow.Pool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BLChaos.Effects;

internal class PoolParty : EffectBase
{
    public PoolParty() : base("Pool Party", 60) { }
    static IEnumerable<AssetPool> pools;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;

        if (pools == null || pools.First() == null) pools = Instances.AllPools;

        if (isNetworked) yield break;
        while (Active)
        {
            AssetPool pool = pools.Random();
            pool.Spawn(GlobalVariables.inFrontOfPlayer, GlobalVariables.lookingAtPlayer, null, true);
            SendNetworkData(Utilities.SerializeInFrontFacingPlayer(), Encoding.ASCII.GetBytes(pool._crate.Barcode.ID));
            yield return new WaitForSeconds(5);
        }
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        string poolBarcode = Encoding.ASCII.GetString(data[1]);

        AssetPool pool = Barcodes.ToAssetPool(poolBarcode);
        if (pool == null)
        {
            Chaos.Warn("Pool not found in client - ID: " + poolBarcode);
            return;
        }

        (Vector3 pos, Quaternion rot) = Utilities.DebytePosRot(data[0]);
        pool.Spawn(pos, rot, null, true);
    }
}
