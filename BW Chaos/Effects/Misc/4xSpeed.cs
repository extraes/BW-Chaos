using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class FourTimesSpeed : EffectBase
    {
        public FourTimesSpeed() : base("4x Speed", 60) { }

        public override void OnEffectStart()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
            Time.timeScale = 4;
        }

        public override void OnEffectEnd()
        {
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
            Time.timeScale = 1;
        }
    }
}
