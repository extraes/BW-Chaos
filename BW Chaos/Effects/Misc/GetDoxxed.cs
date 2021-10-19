﻿using System;
using UnityEngine;
using MelonLoader;
using System.Net;

namespace BWChaos.Effects
{
    //todo: finish
    internal class GetDoxxed : EffectBase
    {
        public GetDoxxed() : base("Get Fucking Doxxed ", 15) { }

        GameObject sign;
        Transform headT;
        float dist = 50;
        float x = 0;
        float y = 0;
        public override void OnEffectStart()
        {
            dist = 50;

            byte[] ipParts = new byte[]
            {
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
            };
            headT = GlobalVariables.Player_PhysBody.rbHead.transform;
            sign = Utilities.SpawnAd(string.Join(".", ipParts));

            sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * 50;

            sign.GetComponent<Rigidbody>().detectCollisions = false;
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
            GameObject.Destroy(sign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
        }
        public override void OnEffectUpdate()
        {
            // move it closer over a period of 5 seconds
            if (dist > 3) dist -= Time.deltaTime * 5; 
            else dist = 3; // god forbid if someone lag spikes on the exact frame that this happens

            x = (float)Math.Cos(dist / 3.125f);
            y = (float)Math.Sin(dist / 3.125f);

            sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * dist;
            if (dist > 4) sign.transform.rotation = Quaternion.Euler(new Vector3(x * 360, y * 360, 0));
            else sign.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(headT.forward, Vector3.up));
        }
        public override void OnEffectEnd()
        {
            
        }
    }
}