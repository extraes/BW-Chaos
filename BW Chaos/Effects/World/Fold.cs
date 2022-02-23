using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.AI;
using PuppetMasta;

namespace BWChaos.Effects
{
    internal class Fold : EffectBase
    {
        public Fold() : base("<b>Fold.</b>") { }

        public override void OnEffectStart()
        {
            try
            {
                foreach (var pm in GameObject.FindObjectsOfType<PuppetMaster>())
                {
                    pm.Kill();
                }
            }
            catch { }
        }
        
    }
}
