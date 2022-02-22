using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class Foggy : EffectBase
    {
        public Foggy() : base("Foggy", 60) { }

        public override void OnEffectStart()
        {
            var fogs = GameObject.FindObjectsOfType<ValveFog>();
            foreach (var fog in fogs)
            {
                MelonCoroutines.Start(ManipFog(fog));
            }
        }
        private IEnumerator ManipFog(ValveFog fog)
        {
            yield return null;
#if DEBUG
            Chaos.Log("Manipulating fog - " + fog.name);
#endif
            float s = fog.startDistance;
            float e = fog.endDistance;
            float t = fog.heightFogThickness;

            fog.startDistance = 0.1f;
            fog.endDistance = 10f;
            fog.heightFogThickness = 0.05f;
            fog.UpdateConstants();

            while (Active) yield return null;

            fog.startDistance = s;
            fog.endDistance = e;
            fog.heightFogThickness = t;
            fog.UpdateConstants();
        }
    }
}
