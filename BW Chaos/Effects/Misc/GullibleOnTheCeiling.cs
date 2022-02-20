using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.RandomShit;
using System.Collections.Generic;
using System.Linq;

namespace BWChaos.Effects
{
    internal class ItSaysGullibleOnTheCeiling : EffectBase
    {
        public ItSaysGullibleOnTheCeiling() : base("It says gullible on the ceiling", 60) { }
        // why a ushort? idk.
        Dictionary<ushort, GameObject> signs = new Dictionary<ushort, GameObject>();
        ushort myID;
        Transform pHead;
        public override void OnEffectStart()
        {
            pHead = GlobalVariables.Player_PhysBody.rbHead.transform;
            var sign = AdManager.CreateNewAd("gullible");
            ushort signID = (ushort)UnityEngine.Random.Range(0, ushort.MaxValue);
            signs.Add(signID, sign);
            myID = signID;
            SendNetworkData(BitConverter.GetBytes(signID).Concat(sign.transform.position.ToBytes()).ToArray());

            GameObject.Destroy(sign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
            var rb = sign.GetComponent<Rigidbody>();
            rb.detectCollisions = false;
            rb.useGravity = false;
        }

        public override void OnEffectUpdate()
        {
            if (signs.TryGetValue(myID, out var sign) && sign != null)
            {
                if (sign == null) return; // In case something goes wrong
                sign.transform.position = pHead.position + Vector3.up * 2;
                sign.transform.rotation = Quaternion.LookRotation(Vector3.up);
                if (Time.frameCount % 4 == 0) SendNetworkData(BitConverter.GetBytes(myID).Concat(sign.transform.position.ToBytes()).ToArray());
            }
        }

        // fucking overbuilt shit to support multiple gullible signs

        public override void HandleNetworkMessage(byte[] data)
        {
            ushort id = BitConverter.ToUInt16(data, 0);
            Vector3 pos = Utilities.DebyteV3(data, sizeof(ushort));

            if (!signs.TryGetValue(id, out var sign))
            {
                // Create a new sign if one with the given ID doesn't already exist
                var newSign = AdManager.CreateNewAd("gullible");
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
                var rb = sign.GetComponent<Rigidbody>();
                rb.detectCollisions = false;
                rb.useGravity = false; // disable gravity because otherwise itll fall in the 4 frame interval

                signs.Add(id, newSign);
                newSign.transform.position = pos;
                newSign.transform.rotation = Quaternion.LookRotation(Vector3.up);
            }
            else sign.transform.position = pos;

            if (!isNetworked) SendNetworkData(data); // broadcast the data to the others
        }

        public override void OnEffectEnd() => signs.Values.ForEach(s => s.Destroy());
    }
}
