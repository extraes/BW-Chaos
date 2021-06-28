using System;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class Placeholder : EffectBase
    {
        public Placeholder() : base("Placeholder Effect") { }

        public override void OnEffectStart() => MelonLogger.Msg("Placeholder start");
        public override void OnEffectUpdate() => MelonLogger.Msg("Placeholder update");
        public override void OnEffectEnd() => MelonLogger.Msg("Placeholder end");
    }
}
