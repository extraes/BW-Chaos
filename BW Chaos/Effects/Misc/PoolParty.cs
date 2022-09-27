using ModThatIsNotMod.Nullables;
using StressLevelZero.Pool;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWChaos.Effects;

internal class PoolParty : EffectBase
{
    public PoolParty() : base("Pool Party", 60) { }
    static Pool[] pools;


    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        if (isNetworked) yield break;

        if (pools == null || pools[0] == null) pools = GameObject.FindObjectsOfType<Pool>().ToArray();

        while (Active)
        {
            Pool pool = pools.Random();
            GameObject spawned = pool.Spawn(Vector3.zero, Quaternion.identity, null, false);
            Utilities.MoveAndFacePlayer(spawned);
            SendNetworkData(spawned.transform.SerializePosRot(), Encoding.ASCII.GetBytes(pool.name));
            spawned.SetActive(true);
            yield return new WaitForSeconds(5);
        }
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        string poolName = Encoding.ASCII.GetString(data[1]);

        Pool pool = pools.FirstOrDefault(p => p.name == poolName);
        if (pool == null)
        {
            Chaos.Warn("Pool not found in client - " + poolName);
            return;
        }

        Poolee poolee = pool.InstantiatePoolee();
        poolee.gameObject.SetActive(true);
        poolee.transform.DeserializePosRot(data[0]);
    }
}
