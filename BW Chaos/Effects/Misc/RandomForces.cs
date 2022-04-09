using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using Random = UnityEngine.Random;
using System.Text;
using System.Linq;

namespace BWChaos.Effects
{
    internal class RandomForces : EffectBase
    {
        public RandomForces() : base("Random Forces", 5, EffectTypes.LAGGY) { }

        public override void HandleNetworkMessage(byte[][] data) 
        {
            Vector3 vec = Utilities.DebyteV3(data[0]);
            string path = Encoding.ASCII.GetString(data[1]);


            var rb = GameObject.Find(path)?.GetComponent<Rigidbody>();
            if (rb == null) Chaos.Warn("Rigidbody not found in client " + path);
            else rb.AddForce(vec * 10);
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            if (isNetworked) yield break;
            bool stagger = true;
            foreach (var rb in GameObject.FindObjectsOfType<Rigidbody>())
            {
                Vector3 vec = Random.onUnitSphere;
                SendNetworkData(vec.ToBytes(), Encoding.ASCII.GetBytes(rb.transform.GetFullPath()));
                rb.AddForce(vec * 10, ForceMode.VelocityChange);
                if (stagger = !stagger) yield return new WaitForFixedUpdate();
            }
        }
    }
}
