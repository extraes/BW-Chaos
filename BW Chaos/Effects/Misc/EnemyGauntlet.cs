using MelonLoader;
using StressLevelZero.AI;
using StressLevelZero.Pool;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// maybe try adding shit from ultrakill?
namespace BWChaos.Effects
{
    internal class EnemyGauntlet : EffectBase
    {
        public EnemyGauntlet() : base("Enemy gauntlet", 90) { }

        private AudioSource soundPlayer = null;
        public override void OnEffectStart()
        {
            // Load sound asset and play it
            if (soundPlayer == null) soundPlayer = GlobalVariables.Player_PhysBody.rbHead.gameObject.AddComponent<AudioSource>();

            if (soundPlayer.clip == null) soundPlayer.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/thecybergrind.mp3");

            soundPlayer.bypassListenerEffects = true;
            soundPlayer.bypassEffects = true;
            soundPlayer.bypassReverbZones = true;
            soundPlayer.ignoreListenerVolume = true;
            soundPlayer.ignoreListenerPause = true;
            soundPlayer.volume = 0.3f;
            soundPlayer.outputAudioMixerGroup = GlobalVariables.MusicMixer;
            soundPlayer.Play();
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            int entangleChange = Prefs.SyncEffects ? 2 : 1;
            Pool nbPool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
            Pool eePool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Ford EarlyExit");
            Pool crabPool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");



            for (int i = 0; i < 4; i++)
            {
                if (i % entangleChange != 0) continue;
                var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                float theta = (i / 8f) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                var spawnedNB = nbPool.InstantiatePoolee(spawnPos, spawnRot);
                spawnedNB.gameObject.SetActive(true);
                yield return new WaitForSeconds(5f);
            }

            yield return new WaitForSeconds(10f);

            for (int i = 0; i < 4; i++)
            {
                if (i % entangleChange != 0) continue;
                var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                float theta = (i / 8f) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                var spawnedCrab = crabPool.InstantiatePoolee(spawnPos, spawnRot);
                spawnedCrab.gameObject.SetActive(true);
                yield return new WaitForSeconds(5f);
            }

            yield return new WaitForSeconds(10f);

            for (int i = 0; i < 4; i++)
            {
                if (i % entangleChange != 0) continue;
                var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                float theta = (i / 8f) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                var spawnedEE = eePool.InstantiatePoolee(spawnPos, spawnRot); //todo: change to earlyexit that can throw stuff
                //spawnedEE.GetComponent<AIBrain>().behaviour.enableThrowAttack = true;
                spawnedEE.gameObject.SetActive(true);
                yield return new WaitForSeconds(5f);
            }

            yield return new WaitForSeconds(10f);
            soundPlayer.volume = 0.1f;
            yield return new WaitForSeconds(10f);
            soundPlayer.volume = 0.05f;
            soundPlayer.Stop();
        }
    }
}