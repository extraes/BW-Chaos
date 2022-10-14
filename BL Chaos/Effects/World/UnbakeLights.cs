using UnityEngine;

namespace BLChaos.Effects;

internal class UnbakeLights : EffectBase
{
    // "fuck you" *unbakes your lights*
    public UnbakeLights() : base("Unbake Lights", 60) { }
    LightmapData[] lightmapData;

    public override void OnEffectStart()
    {
        lightmapData = LightmapSettings.lightmaps;
        LightmapSettings.lightmaps = new LightmapData[0];
    }

    public override void OnEffectEnd()
    {
        LightmapSettings.lightmaps = lightmapData;
    }
}
