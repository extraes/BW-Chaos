using SLZ.Combat;
using SLZ.Marrow.Data;
using SLZ.Player;
using UnityEngine;

namespace BLChaos.Effects;

internal class NoAmmo : EffectBase
{
    public NoAmmo() : base("No Ammo") { }

    public override void OnEffectStart()
    {
        AmmoInventory.Instance.ClearAmmo();
    }
}
