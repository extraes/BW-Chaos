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
    internal class NoDick : EffectBase
    {
        public NoDick() : base("NO DICK.", 30) { }
        private static AudioSource aSource;
        private static AudioClip noDickClip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/no dick.mp3");
        private static AudioClip vineBoom = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/vineboom.mp3");

        public override void OnEffectStart()
        {
            
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            var pHead = Player.GetPlayerHead();
            if (aSource == null)
            {
                aSource = pHead.AddComponent<AudioSource>();
                aSource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
                aSource.volume = 0.2f;
            }
            noDickClip.LoadAudioData();

            aSource.clip = noDickClip;
            aSource.SetScheduledEndTime(0.903);
            aSource.Play();
            yield return null;
            yield return new WaitForSeconds(0.903f);
            aSource.volume *= 1.5f;
            aSource.clip = vineBoom;
            aSource.Play();
        }
        
        public override void OnEffectEnd()
        {
            MelonCoroutines.Start(CoEnd());
        }

        IEnumerator CoEnd()
        {
            GlobalVariables.Player_PhysBody.AddVelocityChange(Vector3.up * 2);
            aSource.volume /= 1.5f;
            aSource.Play();
            yield return new WaitForSeconds(1);
            GlobalVariables.Player_RigManager.EnableBallLoco();
        }
    }
}
