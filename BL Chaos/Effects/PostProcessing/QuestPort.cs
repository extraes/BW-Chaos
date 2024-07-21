using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using Jevil.PostProcessing;

namespace BLChaos.Effects;

internal class QuestPort : EffectBase
{
    // todo (?) : add audio filter?
    public QuestPort() : base("Quest Port", 30, EffectTypes.POST_PROCESS) { }

    [RangePreference(8, 1024, 64)] static int pixels = 128;

    public override void OnEffectStart()
    {
        Material matt = SharedPostProcessingMaterials.Pixelate.Material;
        SharedPostProcessingMaterials.Pixelate.PixelsPerAxis.SetOn(matt, pixels);
        SharedPostProcessingMaterials.Pixelate.Enable();
    }

    public override void OnEffectEnd()
    {
        SharedPostProcessingMaterials.Pixelate.Disable();
    }
}
