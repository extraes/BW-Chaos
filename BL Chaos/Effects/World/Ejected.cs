#if !NOBONELIB
using BoneLib.RandomShit;
using Il2CppSLZ.Marrow.Interaction;

namespace BLChaos.Effects;

internal class Ejected : EffectBase
{
    public Ejected() : base("You Were Sus", 5, EffectTypes.DEFAULT_DISABLED) { }
    GameObject popup;

    public override void OnEffectStart()
    {
        GlobalVariables.Player_RigManager.Teleport(GlobalVariables.Player_PhysRig.transform.position + Vector3.up * 1000, true);
        GlobalVariables.Player_PhysRig.ResetHands(Handedness.BOTH);
        GlobalVariables.Player_PhysRig.AddVelocityChange(GlobalVariables.Player_PhysRig.m_head.forward * 100);
        popup = PopupBoxManager.CreateNewPopupBox(GlobalVariables.Player_RigManager.avatar.name + " was sus");
    }

    public override void OnEffectUpdate()
    {
        Utilities.MoveAndFacePlayer(popup);
    }

    public override void OnEffectEnd()
    {
        GameObject.Destroy(popup);
    }
}
#endif