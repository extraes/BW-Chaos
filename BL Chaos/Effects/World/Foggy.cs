using MelonLoader;
using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class Foggy : EffectBase
{
    public Foggy() : base("Foggy", 60) { }

    public override void OnEffectStart()
    {
        UnhollowerBaseLib.Il2CppArrayBase<ValveFog> fogs = GameObject.FindObjectsOfType<ValveFog>();
        foreach (ValveFog fog in fogs)
        {
            MelonCoroutines.Start(ManipFog(fog));
        }
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
        fog.heightFogThickness = 0.05f;
        fog.UpdateConstants();

        while (Active) yield return null;

        fog.startDistance = s;
        fog.endDistance = e;
        fog.heightFogThickness = t;
        fog.UpdateConstants();
    }
}
