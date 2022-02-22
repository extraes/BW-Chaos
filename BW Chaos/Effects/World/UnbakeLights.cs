using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
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
}
