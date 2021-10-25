using MelonLoader;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class Nearsighted : EffectBase
    {
        public Nearsighted() : base("Nearsighted", 45) { }


        public override void OnEffectStart()
        {
            foreach (Camera cam in GlobalVariables.Cameras) cam.farClipPlane = 10;
        }

        public override void OnEffectEnd()
        {
            foreach (Camera cam in GlobalVariables.Cameras) cam.farClipPlane = 10000;
        }

    }
}
