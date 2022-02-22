using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.AI;

namespace BWChaos.Effects
{
    internal class Aggro : EffectBase
    {
        public Aggro() : base("Aggro", EffectTypes.DONT_SYNC) { }
        static TriggerRefProxy trp;
        
        public override void OnEffectStart()
        {
            trp = trp ?? GameObject.Find("PlayerTrigger").GetComponent<TriggerRefProxy>();
            // try catch because despite the ?. it still nullrefs. idk why
            try
            {
                GameObject.FindObjectsOfType<AIBrain>().ForEach(b => b?.behaviour?.SetAgro(trp));
            }
            catch { }
        }
    }
}
