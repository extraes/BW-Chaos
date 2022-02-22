using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
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
}
