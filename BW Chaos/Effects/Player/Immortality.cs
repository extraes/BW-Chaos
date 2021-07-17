using System;
using UnityEngine;
using MelonLoader;

namespace BWChaos.Effects
{
    internal class Immortality : EffectBase
    {
        public Immortality() : base("Immortality", 60) { }

        public override void OnEffectStart()
        {
            GlobalVariables.Player_Health.healthMode = Player_Health.HealthMode.Invincible;
            GlobalVariables.Player_Health.damageFromAttack = false;
        }

        public override void OnEffectEnd()
        {
            GlobalVariables.Player_Health.healthMode = Player_Health.HealthMode.Mortal;
            GlobalVariables.Player_Health.damageFromAttack = true;
        }
    }
}
