using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class FourTimesSpeed : EffectBase
    {
        public FourTimesSpeed() : base("4x Speed", 30) { }

        public override void OnEffectStart() => GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;

        public override void OnEffectUpdate() => Time.timeScale = 4;

        public override void OnEffectEnd()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
            Time.timeScale = 1;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
        }
    }
}
