using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class FogRoll : EffectBase
{
    public FogRoll() : base("Fog Roll", 60) { }

    public override void OnEffectStart()
    {
        UnhollowerBaseLib.Il2CppArrayBase<ValveFog> fogs = GameObject.FindObjectsOfType<ValveFog>();
        foreach (ValveFog fog in fogs)
        {
            MelonCoroutines.Start(ManipFog(fog));
        }
        //if (fogs.Length == 0)
    }
    private IEnumerator ManipFog(ValveFog fog)
    {
        yield return null;
#if DEBUG
        Log("Manipulating fog - " + fog.name);
#endif
        float s = fog.startDistance;
        float e = fog.endDistance;
        float t = fog.heightFogThickness;

        fog.startDistance = 0.1f;
        fog.endDistance = 10f;
        fog.UpdateConstants();

        const float cycleLength = 2;
        float startTime = 0;
        (float, float) minmax = (0.1f, 20f);
        (float, float) mmDelta = (0.01f, 10f);
        while (Active)
        {
            startTime += Time.deltaTime;

            float ct = Math.Abs((startTime % cycleLength) - (cycleLength / 2)) * 2;
            float cur = minmax.Interpolate(ct);
            float delta = mmDelta.Interpolate(ct);
            fog.startDistance = cur;
            fog.endDistance = delta;
            //fog.startDistance = delta;
            fog.UpdateConstants();

            yield return null;
        }

        fog.startDistance = s;
        fog.endDistance = e;
        fog.UpdateConstants();
    }
}
