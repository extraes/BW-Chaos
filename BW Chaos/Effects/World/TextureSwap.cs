﻿using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class TextureSwap : EffectBase
    {
        static Texture[] textures;
        public TextureSwap() : base("Swap Random Textures") { Init(); }

        private void Init()
        {
            textures = GlobalVariables.ResourcePaths
                .Where(a => a.Contains("/textureswap/"))
                .Select(a => GlobalVariables.EffectResources.LoadAsset<Texture>(a))
                .ToArray();
            // In case IL2 feels like fucking my shit up.
            textures.ForEach(tex => tex.hideFlags = HideFlags.DontUnloadUnusedAsset);
#if DEBUG
            Chaos.Log("Loaded " + textures.Length + " textures into TextureSwap");
#endif
        }

        public override void OnEffectStart()
        {
            #region Initialize
            if (textures == null || textures[0] == null) Init();
            #endregion
            if (isNetworked) return;

            foreach (var mesh in GameObject.FindObjectsOfType<MeshRenderer>())
            {
                if (Random.value < 0.2f)
                {
                    if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                    if (mesh.GetComponent<TMPro.TMP_Text>() == null) continue;

                    Texture tex = textures.Random();
                    mesh.material.SetTexture("_MainTex", tex);
                    SendNetworkData($"{tex.name};{mesh.transform.GetFullPath()}");
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
                return;
            }
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
