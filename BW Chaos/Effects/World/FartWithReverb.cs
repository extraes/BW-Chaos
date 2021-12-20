using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class FartWithReverb : EffectBase
    {
        public FartWithReverb() : base("Fart With Reverb", 5, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }

        private static Transform target;
        private static AudioSource aSource;
        public override void OnEffectStart()
        {
            if (aSource == null) aSource = Player.GetPlayerHead().GetComponent<AudioSource>() ?? Player.GetPlayerHead().AddComponent<AudioSource>();
            if (aSource.clip == null) aSource.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/fart with extra reverb.mp3");
            
            target = GlobalVariables.Player_PhysBody.transform;

            var rbs = GameObject.FindObjectsOfType<Rigidbody>();
            // split it across two ienumerators
            MelonCoroutines.Start(ApplyForces(rbs.Take(rbs.Length / 2)));
            MelonCoroutines.Start(ApplyForces(rbs.Skip(rbs.Length / 2)));
            aSource.Play();
        }

        const float mult = 2f;
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

                rb.AddForce(-Vector3.ClampMagnitude(force * mult, 500 * rb.mass));

                if (stagger = !stagger) yield return new WaitForFixedUpdate();
            }

        }
    }

}
