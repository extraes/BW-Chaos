namespace BLChaos.Effects;

internal class Paralyze : EffectBase
{
    public Paralyze() : base("Paralyze", 20) { }

    float formerVelocity;
    bool formerSlowmo;

    public override void OnEffectStart()
    {
        formerVelocity = GlobalVariables.Player_RigManager.remapHeptaRig.maxVelocity;
        GlobalVariables.Player_RigManager.remapHeptaRig.maxVelocity = 0;
        // no getting around it by jumping or slowing time to stave off an encroaching enemy lol
        GlobalVariables.Player_RigManager.remapHeptaRig.jumpEnabled = false;
        formerSlowmo = TimeManager.slowMoEnabled;
        TimeManager.slowMoEnabled = false;
    }

    public override void OnEffectEnd()
    {
        GlobalVariables.Player_RigManager.remapHeptaRig.maxVelocity = formerVelocity;
        GlobalVariables.Player_RigManager.remapHeptaRig.jumpEnabled = true;
        TimeManager.slowMoEnabled = formerSlowmo;
    }
}
