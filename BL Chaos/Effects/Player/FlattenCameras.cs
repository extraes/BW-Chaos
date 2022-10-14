using UnityEngine;

namespace BLChaos.Effects;

internal class FlattenDepthPerception : EffectBase
{
    public FlattenDepthPerception() : base("Flatten Depth Perception", 30) { }

    public override void OnEffectStart()
    {
        foreach (Camera cam in GlobalVariables.Cameras)
        {
            cam.transform.localScale = new Vector3(0, 1, 1);
            cam.fieldOfView *= 0.75f; // dont think this even works lol
        }
    }
    public override void OnEffectEnd()
    {
        foreach (Camera cam in GlobalVariables.Cameras)
        {
            cam.transform.localScale = Vector3.one;
            cam.fieldOfView /= 0.75f; // dont think this even works lol
        }
    }
}
