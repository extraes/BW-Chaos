using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Reflection;

namespace BWChaos.Effects
{
    internal class FastTimer : EffectBase
    {
        public FastTimer() : base("Fast Effect Timer", 90, EffectTypes.META) { }
        static readonly FieldInfo secInfo = typeof(EffectHandler).GetField("secondsEachEffect", BindingFlags.Instance | BindingFlags.NonPublic);

        public override void OnEffectStart() => secInfo.SetValue(EffectHandler.Instance, 15);
        
        public override void OnEffectEnd() => secInfo.SetValue(EffectHandler.Instance, 30);
    }
}
