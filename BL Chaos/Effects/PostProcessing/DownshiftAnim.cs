using Jevil.PostProcessing;

namespace BLChaos.Effects;

// INHERITS FROM DOWNSHIFT!!!  VVVVVVVVV
internal class DownshiftAnim : Downshift
{
    public DownshiftAnim() : base("Animated", 30, EffectTypes.POST_PROCESS) { }

    public override void OnEffectUpdate()
    {
        Material mat = SharedPostProcessingMaterials.DepthShift.Material;

        float cos = Mathf.Cos(Time.time);
        SharedPostProcessingMaterials.DepthShift.DepthLog.SetOn(mat, cos * cos);
        SharedPostProcessingMaterials.DepthShift.DepthPow.SetOn(mat, MathF.Sin(Time.time));
    }
}
