using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class GravityHands : EffectBase
    {
        public GravityHands() : base("Gravity Hands", 30, EffectTypes.AFFECT_GRAVITY | EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
        [RangePreference(0, 20, 0.5f)] static float gravityMultiplier = 4f;

        public override void OnEffectUpdate() => Physics.gravity = (Player.leftHand.rb.velocity + Player.rightHand.rb.velocity) * gravityMultiplier;
    }
}
