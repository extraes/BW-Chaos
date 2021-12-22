using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class GhostWorld : EffectBase
    {
        public GhostWorld() : base("Ghost World", 60) { }

        public override void OnEffectStart() => GameObject.FindObjectsOfType<ValveCamera>().ForEach(c => c.m_hideAllValveMaterials = true);
        
        public override void OnEffectEnd() => GameObject.FindObjectsOfType<ValveCamera>().ForEach(c => c.m_hideAllValveMaterials = false);
    }
}
