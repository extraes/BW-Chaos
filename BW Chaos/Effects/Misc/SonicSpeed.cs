namespace BWChaos.Effects;

internal class SonicSpeed : EffectBase
{
    public SonicSpeed() : base("SANIC SPEED", 60) { }
    [RangePreference(0, 250, 10)] static readonly float maxAcceleration = 70;
    [RangePreference(0, 200, 10)] static readonly float maxVelocity = 20;

    public override void OnEffectStart()
    {
        // 10x because who doesnt need a little whiplash in life?
        GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = maxAcceleration;
        GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = maxVelocity;
    }

    public override void OnEffectEnd()
    {
        // return base values
        GlobalVariables.Player_RigManager.ControllerRig.maxAcceleration = 7;
        GlobalVariables.Player_RigManager.ControllerRig.maxVelocity = 2;
    }
}
