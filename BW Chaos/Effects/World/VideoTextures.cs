using System;
using UnityEngine;
using UnityEngine.Video;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class VideoTextures : EffectBase
    {
        static GameObject playerParent;
        static VideoClip[] videos;
        static VideoPlayer[] videoPlayers;
        static RenderTexture[] textures;
        public VideoTextures() : base("Video textures", EffectTypes.LAGGY) { Init(); }

        private void Init()
        {
            playerParent = new GameObject("BWChaos Video Parent");
            playerParent.hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(playerParent);

            videos = GlobalVariables.ResourcePaths
                .Where(a => a.Contains("/videoswap/"))
                .Select(a => GlobalVariables.EffectResources.LoadAsset<VideoClip>(a))
                .ToArray();

            videoPlayers = new VideoPlayer[videos.Length];
            textures = new RenderTexture[videos.Length];
            for (int i = 0; i < videoPlayers.Length; i++)
            {
#if DEBUG
                Chaos.Log("Generating videoplayer and rendertexture for " + videos[i].name);
#endif
                videoPlayers[i] = playerParent.AddComponent<VideoPlayer>();
                videoPlayers[i].clip = videos[i];
                videoPlayers[i].isLooping = true;
                textures[i] = new RenderTexture(256, 256, 24);
                textures[i].hideFlags = HideFlags.DontUnloadUnusedAsset;
                textures[i].wrapMode = TextureWrapMode.Repeat;
                GameObject.DontDestroyOnLoad(textures[i]);
                videoPlayers[i].targetTexture = textures[i];
            }
        }

        public override void OnEffectStart() 
        {
            if (playerParent == null || textures == null || textures[0] == null) Init();

            playerParent.SetActive(true);

            if (isNetworked) return;

            foreach (var mesh in GameObject.FindObjectsOfType<MeshRenderer>())
            {
                if (Random.value < 0.2f)
                {
                    if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                    if (mesh.GetComponent<TMPro.TMP_Text>() == null) continue;
                    // 0 because that _should_ be the main texture.
                    mesh.material.SetTexture("_MainTex", textures.Random());
                }
            }
        }
        public override void HandleNetworkMessage(string data)
        {
            string[] args = data.Split(';');
            Texture tex = textures.FirstOrDefault(t => t.name == args[0]);

            var go = GameObject.Find(args[1]);
            if (go == null)
            {
                Chaos.Warn("GameObject was not found in client: " + args[1]);
            }
            else
            {
                if (tex == null)
                {
                    Chaos.Error("Texture was not found in client: " + args[0]);
                    return;
                }
                var mesh = go.GetComponent<MeshRenderer>();
                mesh.material.SetTexture("_MainTex", tex);
            }
        }
    }
}
