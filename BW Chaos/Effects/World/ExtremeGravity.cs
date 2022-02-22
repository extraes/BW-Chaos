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
    internal class ExtremeGravity : EffectBase
    {
        public ExtremeGravity() : base("Extreme Gravity", 30, EffectTypes.AFFECT_GRAVITY) { }

        public override void OnEffectStart() => Physics.gravity = Vector3.down * 50;

        public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
    }
}
