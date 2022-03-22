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
using StressLevelZero.Combat;

namespace BWChaos.Effects
{
    internal class Ricochet : EffectBase
    {
        public Ricochet() : base("Ricochet", 30, EffectTypes.LAGGY) { }
        [EffectPreference("This will make bullets ping off the walls forever. Bad idea if you ask me.")] static bool disableBulletClearing = false;
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

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            if (disableBulletClearing) yield break;

            while (Active)
            {
                // clear lest shit get too fucked
                foreach (var obj in pool.pooledObjects) obj.gameObject.SetActive(false);

                yield return new WaitForSecondsRealtime(5);
            }
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
            //Vector3 angle = Vector3.Reflect(pos, normal);
            Quaternion angleActual = Quaternion.LookRotation(normal);
            var proj = pool.Spawn(pos, angleActual);
            proj._direction = normal;
            proj.gameObject.SetActive(true);

#if DEBUG
            Chaos.Log("Ricocheting bullet at " + pos.ToString() + " to direction " + normal.ToString());
#endif
        }

    }
}