using System.Linq;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Collections.Generic;

namespace BWChaos.Effects
{
    internal class Lube : EffectBase
    {
        public Lube() : base("Lube", 15) { } // SECKS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        private Dictionary<PhysicMaterial, float[]> frictions = new Dictionary<PhysicMaterial, float[]>();
        private Dictionary<Rigidbody, float[]> rbFrictions = new Dictionary<Rigidbody, float[]>();
        //private object CTokenStart, CTokenEnd;

        public override void OnEffectStart() => MelonCoroutines.Start(CoStart());
        public override void OnEffectEnd() => MelonCoroutines.Start(CoEnd());

        private IEnumerator CoStart()
        {
            if (frictions.Count == 0)
            {
                List<string> physMat = new List<string>();
                // get physics materials from the COLLIDERS because cant get them fucking ANYWHERE ELSE
                var mats = Resources.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<Collider>()).Select(c => c.Cast<Collider>().material);
                foreach (var mat in mats) frictions.Add(mat, new float[] { mat.staticFriction, mat.staticFriction2, mat.dynamicFriction, mat.dynamicFriction2 });

                var rbs = GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<Rigidbody>()).Select(m => m.Cast<Rigidbody>()).ToArray();
                foreach (var rb in rbs) rbFrictions.Add(rb, new float[] { rb.drag, rb.drag });
            }

            yield return null;

            var _mats = Resources.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<Collider>()).Select(c => c.Cast<Collider>().material).ToArray();
            for (int i = 0; i < _mats.Length; i++)
            {
                //var mat = frictions.Keys.ElementAt(i);
                var mat = _mats[i];

                mat.staticFriction = 0f;
                mat.staticFriction2 = 0f;
                mat.dynamicFriction = 0f;
                mat.dynamicFriction2 = 0f;
                MelonLogger.Msg("Set values of " + mat.name);

                if (i % 10 == 0) yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < rbFrictions.Count; i++)
            {
                var rb = rbFrictions.Keys.ElementAt(i);

                rb.drag = 0f;
                rb.angularDrag = 0f;

                if (i % 10 == 0) yield return new WaitForEndOfFrame();
            }

            GlobalVariables.Player_RigManager.ControllerRig.maxVelocity *= 100;
            
        }

        private IEnumerator CoEnd()
        {
            for (int i = 0; i < frictions.Count; i++)
            {
                var mat = frictions.Keys.ElementAt(i);
                if (!frictions.TryGetValue(mat, out float[] frics)) continue;

                mat.staticFriction = frics[0];
                mat.staticFriction2 = frics[1];
                mat.dynamicFriction = frics[2];
                mat.dynamicFriction2 = frics[3];

                if (i % 10 == 0) yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < rbFrictions.Count; i++)
            {
                var rb = rbFrictions.Keys.ElementAt(i);
                if (!rbFrictions.TryGetValue(rb, out float[] frics)) continue;

                rb.drag = frics[0];
                rb.angularDrag = frics[1];

                if (i % 10 == 0) yield return new WaitForEndOfFrame();
            }

            GlobalVariables.Player_RigManager.ControllerRig.maxVelocity /= 100;
        }
    }
}
