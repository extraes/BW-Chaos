using System;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class Lag : EffectBase
    {
        public Lag() : base("Lag", 30) { }

        public override void OnEffectStart()
            => GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;

        public override void OnEffectUpdate()
        {
            // todo: cant use extraes' method because i dont think setting timeScale to 0 is a good idea
            MelonLogger.Msg("Placeholder update");
        }

        public override void OnEffectEnd()
            => GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
    }
}
