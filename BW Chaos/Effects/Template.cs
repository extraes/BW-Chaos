using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;

namespace BWChaos.Effects
{
    internal class Template : EffectBase
    {
        public Template() : base("Template Effect") { }

        public override void OnEffectStart() => MelonLogger.Msg("Placeholder start");
        public override void OnEffectUpdate() => MelonLogger.Msg("Placeholder update");
        public override void OnEffectEnd() => MelonLogger.Msg("Placeholder end");
    }
}
