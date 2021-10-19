using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.RandomShit;

namespace BWChaos.Effects
{
    internal class ItSaysGullibleOnTheCeiling : EffectBase
    {
        public ItSaysGullibleOnTheCeiling() : base("It says gullible on the ceiling", 60) { }

        GameObject sign = null;
        Transform pHead;
        public override void OnEffectStart()
        {
            pHead = GlobalVariables.Player_PhysBody.rbHead.transform;
            sign = AdManager.CreateNewAd("gullible");
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
            sign.GetComponent<Rigidbody>().detectCollisions = false;
        }
        public override void OnEffectUpdate()
        {
            if (sign == null) return; // In case something goes wrong
            sign.transform.position = pHead.position + Vector3.up * 2;
            sign.transform.rotation = Quaternion.LookRotation(Vector3.up);
        }
        public override void OnEffectEnd() => GameObject.Destroy(sign);
    }
}
