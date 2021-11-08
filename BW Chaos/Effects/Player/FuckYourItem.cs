using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Player;
using StressLevelZero.Props.Weapons;
using UnityEngine;

//todo: this is broken, it crashes the game iirc
namespace BWChaos.Effects
{
    internal class FuckYourItem : EffectBase
    {
        public FuckYourItem() : base("Fuck Your Items") { }

        public override void OnEffectStart()
        {
            Player.GetObjectInHand(Player.leftHand)?.SetActive(false);
            Player.GetObjectInHand(Player.rightHand)?.SetActive(false);

            foreach (HandWeaponSlotReciever rec in GameObject.FindObjectsOfType<HandWeaponSlotReciever>())
            {
                if (rec.m_SlottedWeapon)
                {
                    rec.m_SlottedWeapon.gameObject.SetActive(false);
                }
            }
        }
    }
}
