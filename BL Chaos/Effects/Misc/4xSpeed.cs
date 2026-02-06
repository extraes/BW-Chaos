namespace BLChaos.Effects;

internal class FourTimesSpeed : EffectBase
{
    public FourTimesSpeed() : base("4x Speed", 30) { }

    public override void OnEffectStart() => GameDisabling.ActivateSlowmo.Add(this);
    public override void OnEffectUpdate() => Time.timeScale = 4;
    public override void OnEffectEnd()
    {
        GameDisabling.ActivateSlowmo.Remove(this);
        Time.timeScale = 1;
    }
}
