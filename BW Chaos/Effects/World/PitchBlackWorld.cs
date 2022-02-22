﻿using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BWChaos.Effects
{
    internal class PitchBlackWorld : EffectBase
    {
        public PitchBlackWorld() : base("Pitch Black World", 30) { }
        Dictionary<MeshRenderer, Material> originalMats = new Dictionary<MeshRenderer, Material>();
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

            foreach (var rend in GameObject.FindObjectsOfType<MeshRenderer>().Where(r => !r.transform.IsChildOfRigManager()))
            {
                originalMats.Add(rend, rend.material);
                rend.material = BLACK;
            }
        }
        
        public override void OnEffectEnd()
        {
            if (originalMats.First().Value == null) return;

            foreach (var kv in originalMats)
            {
                kv.Key.material = kv.Value;
            }
        }
    }
}
