using System.Collections;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects;

internal class RandomForces : EffectBase
{
    public RandomForces() : base("Random Forces", 5, EffectTypes.LAGGY) { }

    public override void HandleNetworkMessage(byte[][] data)
    {
        Vector3 vec = Utilities.DebyteV3(data[0]);
        string path = Encoding.ASCII.GetString(data[1]);


        Rigidbody rb = GameObject.Find(path)?.GetComponent<Rigidbody>();
        if (rb == null) Chaos.Warn("Rigidbody not found in client " + path);
        else rb.AddForce(vec * 10);
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        if (isNetworked) yield break;
        bool stagger = true;
        foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
        {
            Vector3 vec = Random.onUnitSphere;
            SendNetworkData(vec.ToBytes(), Encoding.ASCII.GetBytes(rb.transform.GetFullPath()));
            rb.AddForce(vec * 10, ForceMode.VelocityChange);
            if (stagger = !stagger) yield return new WaitForFixedUpdate();
        }
    }
}
