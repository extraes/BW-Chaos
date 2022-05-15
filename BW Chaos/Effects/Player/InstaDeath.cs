namespace BWChaos.Effects;

internal class InstaDeath : EffectBase
{
    public InstaDeath() : base("Disable Death Slo-Mo", 60) { }

    public override void OnEffectStart()
    {
        GlobalVariables.Player_Health.slowMoOnDeath = false;
        GlobalVariables.Player_Health.ToggleInstantDeathMode(true);
    }
    public override void OnEffectEnd()
    {
        GlobalVariables.Player_Health.slowMoOnDeath = true;
        GlobalVariables.Player_Health.ToggleInstantDeathMode(false);
    }
}
