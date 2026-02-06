#if !NOBONELIB
namespace BLChaos.Effects;

internal class FuckYourItem : EffectBase
{
    public FuckYourItem() : base("Fuck Your Items") { }

    public override void OnEffectStart()
    {
#pragma warning disable UNT0008 // Null propagation on Unity objects
        Player.GetObjectInHand(Player.LeftHand)?.SetActive(false);
        Player.GetObjectInHand(Player.RightHand)?.SetActive(false);
#pragma warning restore UNT0008 // Null propagation on Unity objects

        foreach (SlotContainer container in GameObject.FindObjectOfType<Inventory>().bodySlots)
        {
            container._inventorySlot.DespawnContents();
        }
    }
}

#endif