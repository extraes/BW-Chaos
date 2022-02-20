using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class PointGravity : EffectBase
    {
        public PointGravity() : base("Point Gravity", 30, EffectTypes.AFFECT_GRAVITY) { }
        [RangePreference(0, 10, 0.25f)] static float gravityMultiplier = 1f;

        private Vector3 previousGrav;
        
        public override void OnEffectStart() => previousGrav = Physics.gravity;

        public override void OnEffectUpdate() => Physics.gravity = Player.rightHand.transform.forward.normalized * 12f * gravityMultiplier;

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
