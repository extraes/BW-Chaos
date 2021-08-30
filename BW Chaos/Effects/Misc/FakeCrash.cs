using System;
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
        private AssetBundle soundsBundle = null;
        private AudioSource soundPlayer = null;
        public override void OnEffectStart()
        {
            if (soundsBundle == null)
            {
                string abPath = Path.Combine(Path.GetTempPath(), "BW-Chaos", "winsounds");

                if (!File.Exists(abPath))
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BWChaos.Resources.winsounds"))
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

            if (soundPlayer.clip == null) soundPlayer.clip = soundsBundle.LoadAsset<AudioClip>("assets/w10/windows background.wav");
            // Bypass effects so that it's not too obvious that the sounds are coming from the game 
            soundPlayer.bypassListenerEffects = true;
            soundPlayer.bypassEffects = true;
            soundPlayer.bypassReverbZones = true;
            soundPlayer.ignoreListenerVolume = true;
            soundPlayer.ignoreListenerPause = true;
            soundPlayer.Play();
            //todo: create a new audio stream not tied to the game's entry in volume mixer

            MelonCoroutines.Start(InitiateCrash());
        }

        public override void OnEffectEnd()
        {
        }

        private System.Collections.IEnumerator InitiateCrash ()
        {
            // Sleep the game for 500ms, then wait half the sfx's length to let unity play it
            Il2CppSystem.Threading.Thread.Sleep(500);
            yield return new WaitForSecondsRealtime(soundPlayer.clip.samples / 44100 / 2);
            // then stutter a ranodm amount of time
            Il2CppSystem.Threading.Thread.Sleep((int)(UnityEngine.Random.value * 2000 + 1000));
            yield return new WaitForSecondsRealtime(UnityEngine.Random.value * 100);
            Il2CppSystem.Threading.Thread.Sleep((int)(UnityEngine.Random.value * 4000 + 4000));
        }
    }
}
/* AssetBundle paths
assets/w10/focus0_22050hz.wav
assets/w10/focus1_22050hz.wav
assets/w10/focus2_22050hz.wav
assets/w10/focus3_22050hz.wav
assets/w10/focus4_22050hz.wav
assets/w10/goback_22050hz.wav
assets/w10/hide_22050hz.wav
assets/w10/invoke_22050hz.wav
assets/w10/movenext_22050hz.wav
assets/w10/moveprevious_22050hz.wav
assets/w10/show_22050hz.wav
assets/w10/windows background.wav
assets/w10/windows battery critical.wav
assets/w10/windows battery low.wav
assets/w10/windows critical stop.wav
assets/w10/windows default.wav
assets/w10/windows ding.wav
assets/w10/windows error.wav
assets/w10/windows exclamation.wav
assets/w10/windows feed discovered.wav
assets/w10/windows foreground.wav
assets/w10/windows hardware fail.wav
assets/w10/windows hardware insert.wav
assets/w10/windows hardware remove.wav
assets/w10/windows information bar.wav
assets/w10/windows message nudge.wav
assets/w10/windows notify calendar.wav
assets/w10/windows notify email.wav
assets/w10/windows notify messaging.wav
assets/w10/windows notify system generic.wav
assets/w10/windows user account control.wav
*/