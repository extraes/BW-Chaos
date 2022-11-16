using BoneLib;
using SLZ.Interaction;
using SLZ.Player;
using SLZ.Props.Weapons;
using UnityEngine;

namespace BLChaos.Effects;

internal class FuckYourItem : EffectBase
{
    public FuckYourItem() : base("Fuck Your Items") { }

    public override void OnEffectStart()
    {
#pragma warning disable UNT0008 // Null propagation on Unity objects
        Player.GetObjectInHand(Player.leftHand)?.SetActive(false);
        Player.GetObjectInHand(Player.rightHand)?.SetActive(false);
#pragma warning restore UNT0008 // Null propagation on Unity objects

        foreach (SlotContainer container in GameObject.FindObjectOfType<Inventory>().bodySlots)
        {
            container._inventorySlot.DespawnContents();
        }
    }
}
