using System;
using UnityEngine;
using MelonLoader;

namespace BW_Chaos.Effects
{
    internal class NoRegen : EffectBase
    {
        public NoRegen() : base("No Regen", 120) { }

        private float previousWaitRegen;

        public override void OnEffectStart()
        {
            // todo: using Infinity may not be the best idea but idk
            previousWaitRegen = GlobalVariables.Player_Health.wait_Regen_t;
            GlobalVariables.Player_Health.wait_Regen_t = float.PositiveInfinity;
        }

        public override void OnEffectEnd() => GlobalVariables.Player_Health.wait_Regen_t = previousWaitRegen;
    }
}
