﻿using System;
using UnityEngine;
using MelonLoader;
using System.Net;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class GetDoxxed : EffectBase
    {
        public GetDoxxed() : base("Get Fucking Doxxed ", 15) { }

        private readonly float FPI = (float)Math.PI;
        private GameObject sign;
        private Transform headT;
        private AudioSource soundSource;
        private float dist = 50;
        private float x = 0;
        private float y = 0;
        public override void OnEffectStart()
        {
            if (soundSource == null)
            {
                soundSource = Player.GetPlayerHead().AddComponent<AudioSource>();
                soundSource.bypassEffects = true;
                soundSource.bypassListenerEffects = true;
                soundSource.bypassReverbZones = true;
                soundSource.volume = 0.4f;
            }
            soundSource.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/vineboom.mp3");

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
        private bool wasFarLastFrame; // BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE 
        public override void OnEffectUpdate()
        {
            // move it closer over a period of 5 seconds
            if (dist > FPI) dist -= Time.deltaTime * 5; 
            else dist = FPI; // god forbid if someone lag spikes on the exact frame that this happens

            x = (float)Math.Cos(dist / 3.125f);
            y = (float)Math.Sin(dist / 3.125f);

            sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * dist;
            if (dist > FPI) sign.transform.rotation = Quaternion.Euler(new Vector3(x * 360, y * 360, 0));
            else
            {
                if (wasFarLastFrame) soundSource.Play();
                sign.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(headT.forward, Vector3.up));
            }
            wasFarLastFrame = dist > FPI; // BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE 
        }
        public override void OnEffectEnd()
        {
            
        }
    }
}
