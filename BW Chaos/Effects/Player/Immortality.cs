using System;
using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class Immortality : EffectBase
    {
        public Immortality() : base("Immortality", 60) { }

        public override void OnEffectStart() => GlobalVariables.Player_Health.healthMode = Player_Health.HealthMode.Invincible;
        public override void OnEffectEnd() => GlobalVariables.Player_Health.healthMode = Player_Health.HealthMode.Mortal;
    }
}
