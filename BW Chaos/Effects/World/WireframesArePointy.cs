﻿using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class WireframesArePointy : EffectBase
    {
        public WireframesArePointy() : base("Wireframes are Pointy", 30, EffectTypes.DONT_SYNC) { }
        static Material mat;
        static WireframesArePointy()
        {
            mat = GlobalVariables.EffectResources.LoadAsset<Material>("assets/materials/wireframe.mat");
            mat.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        public override void OnEffectUpdate()
        {
            var hand = Time.frameCount % 2 == 0 ? Player.leftHand : Player.rightHand;
            var dir = hand.transform.forward;

            if (Physics.Raycast(hand.transform.position + dir / 20, dir, out RaycastHit hitInfo, 50f)) {
                var col = hitInfo.collider;
                var rend = col.GetComponentInParent<MeshRenderer>();
                if (rend != null)
                {
                    rend.material = mat;
                }
            }
        }
    }
}