using MelonLoader;
using StressLevelZero.Pool;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

// maybe try adding shit from ultrakill?
namespace BWChaos.Effects
{
    internal class PlayMusic : EffectBase
    {
        public PlayMusic() : base("Play Ultrakill music", 90) { }

        private AssetBundle soundsBundle = null;
        private AudioSource soundPlayer = null;
        public override void OnEffectStart()
        { // lol
            if (soundsBundle == null)
            {
                string abPath = Path.Combine(Path.GetTempPath(), "BW-Chaos", "ultrakillmusic");

                if (!File.Exists(abPath))
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BWChaos.Resources.ultrakillmusic"))
                    {
                        byte[] data;
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            data = ms.ToArray();
                        }
                        File.WriteAllBytes(abPath, data);
                    }
                }
                if (soundsBundle == null) soundsBundle = AssetBundle.LoadFromFile(abPath);
            }
            // Load sound asset and play it
            if (soundPlayer == null) soundPlayer = GlobalVariables.Player_PhysBody.rbHead.gameObject.AddComponent<AudioSource>();

            if (soundPlayer.clip == null) soundPlayer.clip = soundsBundle.LoadAsset<AudioClip>("assets/bundledassets/cybergrind.wav");

            soundPlayer.bypassListenerEffects = true;
            soundPlayer.bypassEffects = true;
            soundPlayer.bypassReverbZones = true;
            soundPlayer.ignoreListenerVolume = true;
            soundPlayer.ignoreListenerPause = true;
            soundPlayer.volume = 0.3f;
            soundPlayer.Play();
            MelonCoroutines.Start(CoRun());
        }

        private IEnumerator CoRun()
        {
            yield return null;

            {
                Pool nullPool = null;
                foreach (var p in GameObject.FindObjectsOfType<Pool>()) if (p.name == "pool - Null Body") nullPool = p;

                for (int i = 0; i < 8; i++)
                {
                    var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                    float theta = (i / 8f) * 360;
                    float x = (float)(Math.Cos(theta * Math.PI / 180));
                    float y = (float)(Math.Sin(theta * Math.PI / 180));

                    Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                    var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                    var spawnedNB = nullPool.InstantiatePoolee(spawnPos, spawnRot);
                    spawnedNB.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitForSeconds(10f);


            {
                Pool crabPool = null;
                foreach (var p in GameObject.FindObjectsOfType<Pool>()) if (p.name == "pool - Crablet") crabPool = p;

                for (int i = 0; i < 8; i++)
                {
                    var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                    float theta = (i / 8f) * 360;
                    float x = (float)(Math.Cos(theta * Math.PI / 180));
                    float y = (float)(Math.Sin(theta * Math.PI / 180));

                    Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                    var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                    var spawnedNB = crabPool.InstantiatePoolee(spawnPos, spawnRot);
                    spawnedNB.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitForSeconds(10f);

            {
                Pool crabPool = null;
                foreach (var p in GameObject.FindObjectsOfType<Pool>()) if (p.name == "pool - Crablet") crabPool = p; //todo: change to earlyexits with projectiles (HOW THOUGH?)
                
                for (int i = 0; i < 8; i++)
                {
                    var playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
                    float theta = (i / 8f) * 360;
                    float x = (float)(Math.Cos(theta * Math.PI / 180));
                    float y = (float)(Math.Sin(theta * Math.PI / 180));

                    Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                    var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
                    var spawnedNB = crabPool.InstantiatePoolee(spawnPos, spawnRot);
                    spawnedNB.gameObject.SetActive(true);
                    yield return new WaitForSeconds(2f);
                }
            }

            yield return new WaitForSeconds(10f);
            soundPlayer.volume = 0.2f;
            yield return new WaitForSeconds(10f);
            soundPlayer.volume = 0.1f;
            soundPlayer.Stop();
        }
    }
}
// assets/bundledassets/cybergrind.wav