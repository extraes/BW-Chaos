namespace BWChaos.Effects;

internal class NoRegen : EffectBase
{
    public NoRegen() : base("No Regen", 180) { }

    private float previousWaitRegen;

    public override void OnEffectStart()
    {
        previousWaitRegen = GlobalVariables.Player_Health.wait_Regen_t;
        GlobalVariables.Player_Health.wait_Regen_t = 420;
    }

    public override void OnEffectEnd()
    {
        GlobalVariables.Player_Health.wait_Regen_t = previousWaitRegen;
        GlobalVariables.Player_Health.SetFullHealth();
    }
}
