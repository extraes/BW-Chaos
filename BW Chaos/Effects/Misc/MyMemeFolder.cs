using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Video;
using System.Diagnostics;

namespace BWChaos.Effects
{
    internal class MyMemeFolder : EffectBase
    {
        public MyMemeFolder() : base("My Meme Folder", 30) { }
        private static string[] vidPaths = GlobalVariables.ResourcePaths.Where(p => p.Contains("video") || p.Contains("memefolder")).ToArray();
        static GameObject vidParent;


        public override void HandleNetworkMessage(byte[][] data)
        {
            // get the data we need out of the byte array array (lol)
            string vidPath = Encoding.ASCII.GetString(data[1]);

            #region Anti fuckup checks

            if (!vidPaths.Contains(vidPath))
            {
                Chaos.Warn("Meme path not found! -> " + vidPath);
                return;
            }

            VideoClip vid = GlobalVariables.EffectResources.LoadAsset<VideoClip>(vidPath);
#if DEBUG
            if (vid == null)
            {
                Chaos.Warn("Video is null, but the path exists in the resources... What?");
            }
#endif
            #endregion

            var sign = Utilities.SpawnAd("");
            // flip it upside down because the render texture is upside down
            sign.transform.DeserializePosRot(data[0]);

            var rend = sign.GetComponentInChildren<MeshRenderer>();
            rend.material = CreateRTex(vid, out VideoPlayer plyr);

            var asource = sign.AddComponent<AudioSource>();
            asource.volume = 0.1f;
            asource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
            plyr.SetTargetAudioSource(0, asource);
            plyr.Play();
            MelonCoroutines.Start(StopVideo(plyr));
        }

        public override void OnEffectStart()
        {
            if (vidParent == null) vidParent = new GameObject("My Meme Folder Video Parent");

            var vidPath = vidPaths.Random();
            var sign = Utilities.SpawnAd("");
            // flip it upside down because the render texture is upside down
            sign.transform.rotation = Quaternion.Euler(sign.transform.rotation.eulerAngles + new Vector3(0, 0, 180));
            SendNetworkData(sign.transform.SerializePosRot(), Encoding.ASCII.GetBytes(vidPath));

            var vid = GlobalVariables.EffectResources.LoadAsset<VideoClip>(vidPath);
            
            var rend = sign.GetComponentInChildren<MeshRenderer>();
            rend.material = CreateRTex(vid, out VideoPlayer plyr);

            var asource = sign.AddComponent<AudioSource>();
            asource.volume = 0.1f;
            asource.outputAudioMixerGroup = GlobalVariables.SFXMixer;
            plyr.SetTargetAudioSource(0, asource);
            plyr.Play();
            MelonCoroutines.Start(StopVideo(plyr));
        }

        private IEnumerator StopVideo(VideoPlayer player)
        {
            if (player.clip.length > 5)
            {
                player.isLooping = true;
                yield return new WaitForSeconds(5);
                player.isLooping = false;
                yield break;
            }

            yield return new WaitForSeconds(Duration);
            player.Stop();
        }

        private Material CreateRTex(VideoClip clip, out VideoPlayer plyr)
        {
            var mat = new Material(Shader.Find("Valve/vr_standard"));
            var tex = new RenderTexture(256, 256, 24);
            mat.mainTexture = tex;

            var player = vidParent.AddComponent<VideoPlayer>();
            player.clip = clip;
            player.targetTexture = tex;
            plyr = player;

            return mat;
        }
    }
}
