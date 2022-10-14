using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;
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

        foreach (HandWeaponSlotReciever rec in GameObject.FindObjectsOfType<HandWeaponSlotReciever>())
        {
            if (rec.m_SlottedWeapon)
            {
                rec.m_SlottedWeapon.gameObject.SetActive(false);
            }
        }
    }
}
