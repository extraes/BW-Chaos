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

        public override void OnEffectStart() => MelonLoader.MelonCoroutines.Start(CoRun());

        public IEnumerator CoRun ()
        {
            int[] arr = new int[] { -1, 1 };
            Physics.gravity = new Vector3(9.8f * 3 * arr[Random.RandomRange(0, 2)], 9.8f * 6, 9.8f * 3 * arr[Random.RandomRange(0, 2)]);
            yield return new WaitForSecondsRealtime(2);
            Physics.gravity = new Vector3(0, -9.8f, 0);

        }
    }
}
