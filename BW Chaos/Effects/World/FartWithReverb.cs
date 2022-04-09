using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class FartWithReverb : EffectBase
    {
        public FartWithReverb() : base("Fart With Reverb", 5, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
        [RangePreference(0.25f, 10, 0.25f)] static float forceMultiplier = 2f;

        private static Transform target;
        private static AudioClip clip;
        public override void OnEffectStart()
        {
            clip = clip != null ? clip : GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/fart with extra reverb.mp3");
            target = GlobalVariables.Player_PhysBody.transform;

            var rbs = GameObject.FindObjectsOfType<Rigidbody>().ToList();
            // match CPU count, not thread count, and higher numbers lag spike the game longer
            int perCPUCount = 2 * rbs.Count / SystemInfo.processorCount;
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            // split it across multiple ienumerators
            for (int i = 0; i < SystemInfo.processorCount / 2; i++)
            {
                int min = 2 * i * rbs.Count / SystemInfo.processorCount;
                var rbPart = rbs.GetRange(min, perCPUCount);
                MelonCoroutines.Start(ApplyForces(rbPart));
#if DEBUG
                Chaos.Log($"Started {nameof(ApplyForces)} with {perCPUCount} rigidbodies starting at idx {min} idxs=({min}-{min+perCPUCount}) because there are {SystemInfo.processorCount} CPUs");
#endif
            }
#if DEBUG
            sw.Stop();
            Chaos.Log("Finished starting coroutines in " + sw.ElapsedMilliseconds + "ms");
#endif

            GlobalVariables.SFXPlayer.Play(clip);
        }

        private IEnumerator ApplyForces(IEnumerable<Rigidbody> rbs)
        {
            yield return null;
            bool stagger = false;
            foreach (var rb in rbs)
            {
                if (!Active || rb is null) yield break;

                // ignore it if its far away
                if ((target.position - rb.transform.position).sqrMagnitude > 30 * 30) continue;

                float dt = Time.fixedDeltaTime;
                Vector3 p = rb.transform.position; //our current position
                Vector3 v = rb.velocity; //our current velocity
                // subt V3.up because then rb's wont try to go into the floor   V
                Vector3 force = rb.mass * (target.transform.position - Vector3.up - p - v * dt) / (dt);

                rb.AddForce(-Vector3.ClampMagnitude(force * forceMultiplier, 500 * rb.mass));

                if (stagger = !stagger) yield return new WaitForFixedUpdate();
            }

        }
    }

}
