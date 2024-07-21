using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using Jevil.PostProcessing;

namespace BLChaos.Effects;

internal class Downshift : EffectBase
{
    protected Downshift(string variantName, int duration, EffectTypes types) : base("Downshift - " + variantName, duration, types) { }
    public Downshift() : base("Downshift", 30, EffectTypes.POST_PROCESS) { }

    public override void OnEffectStart()
    {
        Material matt = SharedPostProcessingMaterials.DepthShift.Material;
        SharedPostProcessingMaterials.DepthShift.DepthLog.ResetOn(matt);
        SharedPostProcessingMaterials.DepthShift.DepthPow.ResetOn(matt);
        SharedPostProcessingMaterials.DepthShift.ReverseDepth.ResetOn(matt);
        SharedPostProcessingMaterials.DepthShift.Enable();
    }

    public override void OnEffectEnd()
    {
        SharedPostProcessingMaterials.DepthShift.Disable();
    }
}
