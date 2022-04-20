using MelonLoader;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects;

internal class Fling : EffectBase
{
    public Fling() : base("Fling Everything", EffectTypes.AFFECT_GRAVITY) { }
    readonly int[] arr = new int[] { -1, 1 };
    [RangePreference(0, 10, 0.125f)] static readonly float forceMultiplier = 1;

    public override void OnEffectStart()
    {
        if (isNetworked) return;
        Vector3 newGrav = new Vector3(9.8f * 3 * arr.Random(), 9.8f * 6, 9.8f * 3 * arr.Random()) * forceMultiplier;
        SendNetworkData(newGrav.ToBytes());
        MelonCoroutines.Start(DoGravity(newGrav));
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        Vector3 grav = Utilities.DebyteV3(data);
        MelonCoroutines.Start(DoGravity(grav));
    }

    public IEnumerator DoGravity(Vector3 grav)
    {
        Physics.gravity = grav;
        yield return new WaitForSecondsRealtime(2);
        Physics.gravity = new Vector3(0, -9.8f, 0);

    }
}
