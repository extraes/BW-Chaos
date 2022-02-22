using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;

namespace BWChaos.Effects
{
    internal class FlattenDepthPerception : EffectBase
    {
        public FlattenDepthPerception() : base("Flatten Depth Perception", 30) { }

        public override void OnEffectStart()
        {
            foreach (var cam in GlobalVariables.Cameras)
            {
                cam.transform.localScale = new Vector3(0, 1, 1);
                cam.fieldOfView *= 0.75f; // dont think this even works lol
            }
        }
        public override void OnEffectEnd()
        {
            foreach (var cam in GlobalVariables.Cameras)
            {
                cam.transform.localScale = Vector3.one;
                cam.fieldOfView /= 0.75f; // dont think this even works lol
            }
        }
    }
}
