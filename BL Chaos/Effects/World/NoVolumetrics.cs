namespace BLChaos.Effects;

internal class NoVolumetrics : EffectBase
{
    public NoVolumetrics() : base("Fog-B-Gone", 60) { }

    VolumetricRendering volRen;

    public override void OnEffectStart()
    {
        volRen = GameObject.FindObjectOfType<VolumetricRendering>();
        volRen.disable();
    }

    public override void OnEffectEnd()
    {
        if (volRen == null)
            volRen = GameObject.FindObjectOfType<VolumetricRendering>();

        volRen.enable();
    }
}
