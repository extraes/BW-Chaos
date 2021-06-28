using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BW_Chaos.Effects
{
    internal class PointGravity : EffectBase
    {
        public PointGravity() : base("Point Gravity", 30) { }

        private Vector3 previousGrav;

        public override void OnEffectStart() => previousGrav = Physics.gravity;

        public override void OnEffectUpdate() => Physics.gravity = Player.rightHand.transform.forward.normalized * 12f;

        public override void OnEffectEnd() => Physics.gravity = previousGrav;
    }
}
