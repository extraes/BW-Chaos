using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using MelonLoader.Assertions;

namespace BWChaos.Effects
{
    internal class WhiteChristmas : EffectBase
    {
        public WhiteChristmas() : base("White Christmas", 30, EffectTypes.DONT_SYNC) { }
        static Shader shader = Shader.Find("Valve/vr_standard");
        static Material mat;

        // static initializer cause otherwise i cant set hideflags
        static WhiteChristmas()
        {
            mat = new Material(shader);
            mat.hideFlags = HideFlags.DontUnloadUnusedAsset;
            mat.color = Color.white;
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
