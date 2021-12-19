﻿using System;
using System.Reflection;
using UnityEngine;
using MelonLoader;
using System.IO;
using UnityEngine.UI;

namespace BWChaos.Effects
{
    internal class FakeCrash : EffectBase
    {
        public FakeCrash() : base("Fake crash", 7, EffectTypes.HIDDEN) { }
        private static AudioSource soundPlayer = null;
        private int sleepLength;
        public override void OnEffectStart()
        {
            // Load sound asset and play it
            if (soundPlayer == null) soundPlayer = GlobalVariables.Player_PhysBody.rbHead.gameObject.AddComponent<AudioSource>();
            
            if (soundPlayer.clip == null) soundPlayer.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/windowsbackground.wav");

            sleepLength = UnityEngine.Random.RandomRange(4, 8) * 1000;
            SendNetworkData(sleepLength.ToString());

            // Bypass effects so that it's not too obvious that the sounds are coming from the game 
            soundPlayer.bypassListenerEffects = true;
            soundPlayer.bypassEffects = true;
            soundPlayer.bypassReverbZones = true;
            soundPlayer.ignoreListenerVolume = true;
            soundPlayer.ignoreListenerPause = true;
            soundPlayer.Play();

            MelonCoroutines.Start(InitiateCrash());
        }

        public override void HandleNetworkMessage(string data)
        {
            sleepLength = int.Parse(data);
        }

        [AutoCoroutine]
        public System.Collections.IEnumerator InitiateCrash ()
        {
            yield return null;
            // Sleep the game for 500ms, then wait half the sfx's length to let unity play it
            Il2CppSystem.Threading.Thread.Sleep(500);
            yield return new WaitForSecondsRealtime(soundPlayer.clip.samples / 44100 / 2);
            // then stutter a ranodm amount of time
            Il2CppSystem.Threading.Thread.Sleep(sleepLength);
        }
    }
}