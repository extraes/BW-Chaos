using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects;

internal class PitchBlackWorld : EffectBase
{
    public PitchBlackWorld() : base("Pitch Black World", 30) { }

    readonly Dictionary<MeshRenderer, Material> originalMats = new Dictionary<MeshRenderer, Material>();
    static Material BLACK;


    public override void OnEffectStart()
    {
        originalMats.Clear();
        if (BLACK == null)
        {
            BLACK = new Material(Shader.Find("SLZ/Fill Color"));
            BLACK.color = Color.black;
            //BLACK.mainTexture = new Texture2D(69, 420, TextureFormat.RGB24, false);
            BLACK.hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(BLACK);
        }

        foreach (MeshRenderer rend in GameObject.FindObjectsOfType<MeshRenderer>().Where(r => !r.transform.IsChildOfRigManager()))
        {
            originalMats.Add(rend, rend.material);
            rend.material = BLACK;
        }
    }

    public override void OnEffectEnd()
    {
        if (originalMats.First().Value == null) return;

        foreach (KeyValuePair<MeshRenderer, Material> kv in originalMats)
        {
            kv.Key.material = kv.Value;
        }
    }
}
