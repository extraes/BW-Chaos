using System;
using System.Linq;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class SpawnJevil : EffectBase
    {
        public SpawnJevil() : base("Spawn him.") { }

        AudioSource soundSource;
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
            soundSource.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/jevil_chaoschaos.wav");
            
            var pool = GameObject.FindObjectsOfType<StressLevelZero.Pool.Pool>().FirstOrDefault(p => p.name == "pool - Jevil");
            var jevil = pool.InstantiatePoolee(Vector3.zero, Quaternion.identity).gameObject;
            var head = Player.GetPlayerHead().transform;
            
            jevil.transform.position = head.position + head.forward;
            jevil.transform.rotation = Quaternion.LookRotation(-Vector3.ProjectOnPlane(head.forward, Vector3.up));
            jevil.SetActive(true);
            soundSource.Play();
        }

    }
}
