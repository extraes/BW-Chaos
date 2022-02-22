using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BWChaos.Effects
{
    internal class ExplosiveGuns : EffectBase
    {
        public ExplosiveGuns() : base("Explosive Guns", 60) { }
        [RangePreference(0.125f, 5, 0.125f)] static float forceMultiplier = 1;

        public override void OnEffectStart() => Hooking.OnPostFireGun += Hooking_OnPostFireGun;

        public override void OnEffectEnd() => Hooking.OnPostFireGun -= Hooking_OnPostFireGun;

        private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
        {
            var origin = obj.firePointTransform.position;

            var cols = Physics.OverlapSphere(origin, 2);
            var rmn = GlobalVariables.Player_RigManager.name;
            var rbs = cols.Where(c => !c.transform.InHierarchyOf(rmn)).Select(c => c.attachedRigidbody).Distinct();

            MelonCoroutines.Start(ApplyForces(rbs.Skip(rbs.Count() / 2), origin));
            MelonCoroutines.Start(ApplyForces(rbs.Take(rbs.Count() / 2), origin));
        }

        // yoinking code from fartwithreverb lol
        private IEnumerator ApplyForces(IEnumerable<Rigidbody> rbs, Vector3 origin)
        {
            yield return null;
            bool stagger = false;
            foreach (var rb in rbs)
            {
                if (rb == null) continue;
                Vector3 p = rb.transform.position; //our current position
                Vector3 v = rb.velocity; //our current velocity
                Vector3 force = rb.mass * (origin - p - v * Time.deltaTime) / Time.deltaTime;

                rb.AddForce(-Vector3.ClampMagnitude(force * forceMultiplier, 500 * rb.mass));

                if (stagger = !stagger) yield return new WaitForFixedUpdate();
            }
        }
    }
}
