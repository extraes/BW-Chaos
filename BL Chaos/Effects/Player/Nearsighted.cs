using UnityEngine;

namespace BLChaos.Effects;

internal class Nearsighted : EffectBase
{
    public Nearsighted() : base("Nearsighted", 45) { }
    [RangePreference(1, 25, 1)] static readonly int viewDistance = 10;

    public override void OnEffectStart()
    {
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>()) cam.farClipPlane = viewDistance;
    }

    public override void OnEffectEnd()
    {
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>()) cam.farClipPlane = 10000;
    }

}
