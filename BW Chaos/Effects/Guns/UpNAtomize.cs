using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.Pool;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BWChaos.Effects
{
    internal class UpNAtomize : EffectBase
    {
        public UpNAtomize() : base("Up-N-Atomize", 90, EffectTypes.LAGGY) { }
        [RangePreference(0, 10, 0.25f)] static float radius = 2f;
        [RangePreference(0, 50, 0.5f)] static float forceMultiplier = 1f;
        [RangePreference(1, 10, 1)] static int rbsPerFrame = 4;
        
        ProjectilePool pool;
        Action<Collider, Vector3, Vector3> onBulletHit;

        public override void OnEffectStart()
        {
            pool = GameObject.FindObjectOfType<ProjectilePool>();
            Hooking.OnPostFireGun += Hooking_OnPostFireGun;
            onBulletHit += OnBulletHit;
        }


        public override void OnEffectEnd()
        {
            Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
        }

        private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
        {
            pool.lastSpawn.onCollision.AddListener(onBulletHit);
            MelonCoroutines.Start(RemoveListener(pool.lastSpawn.onCollision));
        }
        private IEnumerator RemoveListener(UnityEvent<Collider, Vector3, Vector3> unityEvent)
        {
            yield return new WaitForSeconds(1);
            unityEvent?.RemoveListener(onBulletHit);
        }
        private void OnBulletHit(Collider col, Vector3 pos, Vector3 normal)
        {
            //Chaos.Log("Bullet collided with " + col.name);
            //Chaos.Log("Vector3 argument 2 = " + pos.ToString());
            //Chaos.Log("Vector3 argument 3 = " + normal.ToString());
            var cols = Physics.OverlapSphere(pos, radius * 1.25f);
            MelonCoroutines.Start(ApplyForces(cols.Take(cols.Count / 2), pos));
            MelonCoroutines.Start(ApplyForces(cols.Skip(cols.Count / 2), pos));
        }

        private IEnumerator ApplyForces(IEnumerable<Collider> cols, Vector3 origin)
        {
            yield return null;
            int counter = 0;
            foreach (var col in cols)
            {
                if (col == null || col.attachedRigidbody == null || col.attachedRigidbody.isKinematic) continue;
                var rb = col.attachedRigidbody;
                rb.AddExplosionForce(rb.mass * forceMultiplier * 25, origin, radius, 2);
                if (counter++ % rbsPerFrame == 0) yield return null;
            }
        }


    }
}
