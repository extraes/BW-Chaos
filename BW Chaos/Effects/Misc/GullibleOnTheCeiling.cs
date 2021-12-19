using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.RandomShit;
using System.Collections.Generic;

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
            ushort signID = (ushort)UnityEngine.Random.Range(0,ushort.MaxValue);
            signs.Add(signID, sign);
            myID = signID;
            SendNetworkData($"{signID};{sign.transform.position.Serialize(4).Join()}");

            GameObject.Destroy(sign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
            sign.GetComponent<Rigidbody>().detectCollisions = false;
        }
        public override void OnEffectUpdate()
        {
            if (signs.TryGetValue(myID, out var sign) && sign != null)
            {
                if (sign == null) return; // In case something goes wrong
                sign.transform.position = pHead.position + Vector3.up * 2;
                sign.transform.rotation = Quaternion.LookRotation(Vector3.up);
                if (Time.frameCount % 4 == 0) SendNetworkData($"{myID};{sign.transform.position.Serialize(4).Join()}");
            }
        }

        // fucking overbuilt shit to support multiple gullible signs
        public override void HandleNetworkMessage(string data)
        {
            string[] datas = data.Split(';');

            ushort id = ushort.Parse(datas[0]);
            Vector3 pos = Utilities.DeserializeV3(datas[1]);
            if (!signs.TryGetValue(id, out var sign))
            {
                var newSign = AdManager.CreateNewAd("gullible");
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
                GameObject.Destroy(newSign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
                sign.GetComponent<Rigidbody>().detectCollisions = false;

                signs.Add(id, newSign);
                newSign.transform.position = pos;
            }
            else sign.transform.position = pos;
        }

        public override void OnEffectEnd() => signs.Values.ForEach(s => s.Destroy());
    }
}
