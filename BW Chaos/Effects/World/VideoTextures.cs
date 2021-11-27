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
        GameObject playerParent;
        VideoClip[] videos;
        VideoPlayer[] videoPlayers;
        RenderTexture[] textures;
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
                MelonLogger.Msg("Generating videoplayer and rendertexture for " + videos[i].name);
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
    }
}
