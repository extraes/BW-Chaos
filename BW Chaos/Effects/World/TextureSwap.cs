using MelonLoader;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class TextureSwap : EffectBase
    {
        Texture[] textures;
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
            MelonLogger.Msg("Loaded " + textures.Length + " textures into TextureSwap");
#endif
        }

        public override void OnEffectStart()
        {
            #region Initialize
            if (textures == null || textures[0] == null) Init();
            #endregion

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
