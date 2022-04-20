using ModThatIsNotMod;
using UnityEngine;

namespace BWChaos.Effects;

internal class Ejected : EffectBase
{
    public Ejected() : base("You Were Sus", EffectTypes.DEFAULT_DISABLED) { }

    public override void OnEffectStart()
    {
        GlobalVariables.Player_RigManager.Teleport(GlobalVariables.Player_PhysBody.transform.position + Vector3.up * 1000, true);
        GlobalVariables.Player_PhysBody.AddVelocityChange(Player.GetPlayerHead().transform.forward * 100);
        Notifications.SendNotification("Ford was ejected", 5);
    }
}
