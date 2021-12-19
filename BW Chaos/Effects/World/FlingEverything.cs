using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class Fling : EffectBase
    {
        public Fling() : base("Fling Everything", EffectTypes.AFFECT_GRAVITY) { }
        int[] arr = new int[] { -1, 1 };

        public override void OnEffectStart()
        {
            if (isNetworked) return;
            Vector3 newGrav = new Vector3(9.8f * 3 * arr.Random(), 9.8f * 6, 9.8f * 3 * arr.Random());
            SendNetworkData(newGrav.Serialize(2).Join());
            MelonCoroutines.Start(DoGravity(newGrav));
        }

        public override void HandleNetworkMessage(string data)
        {
            Vector3 grav = Utilities.DeserializeV3(data);
            MelonCoroutines.Start(DoGravity(grav));
        }

        public IEnumerator DoGravity (Vector3 grav)
        {
            Physics.gravity = grav;
            yield return new WaitForSecondsRealtime(2);
            Physics.gravity = new Vector3(0, -9.8f, 0);

        }
    }
}
