using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class FogRoll : EffectBase
{
    public FogRoll() : base("Fog Roll", 60) { }
    [RangePreference(0.5f, 5, 0.5f)] static float interval = 2;

    public override void OnEffectStart()
    {
        if (isNetworked) return;

        MelonCoroutines.Start(CoRun(interval));
        SendNetworkData(BitConverter.GetBytes(interval));
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        float netInterval = BitConverter.ToSingle(data, 0);
        MelonCoroutines.Start(CoRun(netInterval));
    }

    private IEnumerator CoRun(float interval)
    {
        yield return null;
        VolumetricRendering volRen = GameObject.FindObjectOfType<VolumetricRendering>();
        bool toggle = false;

        while(Active)
        {
            if (toggle = !toggle) volRen.enable();
            else volRen.disable();

            yield return new WaitForSeconds(interval);
        }
    }
}
