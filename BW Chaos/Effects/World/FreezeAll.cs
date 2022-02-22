using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class FreezeAll : EffectBase
    {
        public FreezeAll() : base("Freeze Everything") { }

        public override void OnEffectStart()
        {
            try
            {
                foreach (var rb in GameObject.FindObjectsOfType<Rigidbody>())
                {
                    if (rb == null || rb.transform.IsChildOfRigManager()) continue;
                    rb?.Sleep();
                }
            }
            catch { }
        }
    }
}
