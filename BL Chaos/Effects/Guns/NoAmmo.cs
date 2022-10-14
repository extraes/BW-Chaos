using StressLevelZero.Combat;
using StressLevelZero.Player;
using UnityEngine;

namespace BLChaos.Effects;

internal class NoAmmo : EffectBase
{
    public NoAmmo() : base("No Ammo") { }

    public override void OnEffectStart()
    {
        // if it doesnt work even after doing this im going to fucking murder someone.
        PlayerInventory inventory = GameObject.FindObjectOfType<PlayerInventory>();
        inventory.AddAmmo(Weight.LIGHT, -inventory.GetAmmo(Weight.LIGHT));
        inventory.AddAmmo(Weight.MEDIUM, -inventory.GetAmmo(Weight.MEDIUM));
        inventory.AddAmmo(Weight.HEAVY, -inventory.GetAmmo(Weight.HEAVY));
        inventory.RemoveAmmo(Weight.LIGHT, inventory._lightAmmo);
        inventory.RemoveAmmo(Weight.MEDIUM, inventory._mediumAmmo);
        inventory.RemoveAmmo(Weight.HEAVY, inventory._heavyAmmo);
        inventory.ClearAmmo();
    }
}
