using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Interaction;

namespace BWChaos.Effects
{
    internal class BarrySteakfries : EffectBase
    {
        public BarrySteakfries() : base("Barry Steakfries", 30) { }

        public override void OnEffectStart()
        {
            //todo: are these two even necessary?
            Hooking.OnGripAttached += OnGrab;
            Hooking.OnGripDetached += OnDrop;
            // If the player has a gun in their hand when the effect activates
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.rightHand);
            OnGrab(GameObject.FindObjectOfType<Grip>(), Player.leftHand);

            Hooking.OnPostFireGun += OnFire;
        }

        public override void OnEffectEnd()
        {
            Hooking.OnGripAttached -= OnGrab;
            Hooking.OnGripDetached -= OnDrop;

            Hooking.OnPostFireGun -= OnFire;
        }

        Gun rightGun;
        Gun leftGun;
        private void OnGrab(Grip _, Hand hand)
        {
            if (hand?.attachedInteractable?.GetComponentInParent<MagazineSocket>() != null)
            {
                Gun gun;
                if (hand == Player.rightHand)
                {
                    gun = Player.GetGunInHand(hand);
                    rightGun = gun;
                }
                else
                {
                    gun = Player.GetGunInHand(hand);
                    leftGun = gun;
                }
                gun.kickForce *= 20;
                gun.muzzleVelocity *= 200;
                gun.magazineSocket.isInfiniteAmmo = true;
            }
        }
        private void OnDrop(Grip _, Hand hand)
        {
            if (hand == Player.rightHand)
            {
                if (rightGun != null)
                {
                    rightGun.kickForce *= 20;
                    rightGun.muzzleVelocity /= 200;
                    rightGun.magazineSocket.isInfiniteAmmo = false;
                    rightGun = null;
                }
            }
            else
            {
                if (leftGun != null)
                {
                    leftGun.kickForce *= 20;
                    leftGun.muzzleVelocity /= 200;
                    leftGun.magazineSocket.isInfiniteAmmo = false;
                    leftGun = null;
                }
            }
        }

        private void OnFire(Gun gun)
        {
            GlobalVariables.Player_PhysBody.AddVelocityChange(-gun.transform.forward * 5);
        }

    }
}
