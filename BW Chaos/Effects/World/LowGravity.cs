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
    internal class LowGravity : EffectBase
    {
        // dont mark as "use gravity" because it doesnt drastically fuck gravity, causing everything to reevaluate if it should move
        public LowGravity() : base("Low Gravity", 120) { }

        public override void OnEffectStart() => Physics.gravity = new Vector3(0, -1, 0);
        public override void OnEffectEnd() => Physics.gravity = new Vector3(0, -9.81f, 0);
    }
}
