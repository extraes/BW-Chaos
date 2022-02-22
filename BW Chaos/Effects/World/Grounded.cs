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
    internal class Grounded : EffectBase
    {
        public Grounded() : base("Grounded", 60) { }

        public override void OnEffectUpdate() => GlobalVariables.Player_PhysBody.AddImpulseForce(Vector3.down * Time.deltaTime * 2000);
        
    }
}
