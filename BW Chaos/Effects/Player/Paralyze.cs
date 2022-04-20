namespace BWChaos.Effects;

internal class Paralyze : EffectBase
{
    public Paralyze() : base("Paralyze", 20) { }

    public override void OnEffectStart()
    {
        GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 0;
        // no getting around it by jumping or slowing time to stave off an encroaching enemy lol
        GlobalVariables.Player_RigManager.ControllerRig.jumpEnabled = false;
        GlobalVariables.Player_RigManager.ControllerRig.slowMoEnabled = false;
    }

    public override void OnEffectEnd()
    {
        GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 2;
        GlobalVariables.Player_RigManager.ControllerRig.jumpEnabled = true;
        GlobalVariables.Player_RigManager.ControllerRig.slowMoEnabled = false;
    }
}
