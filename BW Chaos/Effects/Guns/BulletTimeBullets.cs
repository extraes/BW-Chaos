using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.Rig;
using Chronos;
using StressLevelZero.Pool;
using StressLevelZero.Combat;

namespace BWChaos.Effects
{
    [DontRegisterEffect] // shits broken rn. idk how to use chronos in local mode at runtime
    internal class BulletTimeBullets : EffectBase
    {
        // as with inaccurateguns, this will fuck shit up due to latency, so dont sync
        public BulletTimeBullets() : base("Bullet Time Bullets", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
        private static AreaClock3D clock = null;
        private static bool isBTBAlreadyActive = false; // We don't want two BTBs at the same time. That would be bad.

        // I MISS THE RAGE !?!?!?!?!?!?!?!? (i thought this was funny.)
        public override void HandleNetworkMessage(string data) => "I MISS THE RAGE⁉️⁉️⁉️⁉️⁉️".Trim();
        public override void OnEffectStart()
        {
            // do the same Chronos setup shit as in Effects/Misc/Rewind.cs
            if (isBTBAlreadyActive)
            {
                Chaos.Warn("Two instances of BTB can't be active at the same time! Not running again!");
            }

            isBTBAlreadyActive = true;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;

            #region Set up Chronos & clock

            GameObject chronos = GameObject.Find("ChronosController") ?? new GameObject("ChronosController");
            if (clock == null) clock = chronos.AddComponent<AreaClock3D>();

            //                  V can't do ?., so we try catch.
            try { if (Timekeeper.instance == null) chronos.AddComponent<Timekeeper>(); }
            catch (System.Exception err)
            {
#if DEBUG
                Chaos.Warn("Caught Singleton exception, adding Timekeeper now. In case you wanted the error, here:");
                Chaos.Warn(err);
#endif
                chronos.AddComponent<Timekeeper>();
            }

            #endregion

            #region Register Rb's into clock

            foreach (Rigidbody rb in Utilities.FindAll<Rigidbody>())
            {
                if (rb.transform.IsChildOfRigManager() /*|| rb.IsSleeping()*/ || rb.isKinematic) continue;

                // Ignore rigidbodies that are a part of the body
                Timeline timeline = rb.GetComponent<Timeline>();
                if (timeline == null)
                    timeline = rb.gameObject.AddComponent<Timeline>();
                else
                    continue;
                timeline.mode = TimelineMode.Local;
                timeline._clock = clock;
            }

            #endregion

            #region Register animators into clock

            foreach (Animator anim in Utilities.FindAll<Animator>())
            {
                // Ignore animators that are a part of the body
                if (anim.GetComponentInParent<RigManager>()) continue;

                Timeline timeline = anim.GetComponent<Timeline>();
                if (timeline == null)
                    timeline = anim.gameObject.AddComponent<Timeline>();
                else
                    continue;
                timeline.mode = TimelineMode.Local;
                timeline._clock = clock;
            }

            #endregion

            try { while (GlobalVariables.ActiveEffects.Any(e => e.Name == "4x speed")) GlobalVariables.ActiveEffects.FirstOrDefault(e => e.Name == "4x speed")?.ForceEnd(); } catch { }
            Hooking.OnPostFireGun += Hooking_OnPostFireGun;
        }


        public override void OnEffectEnd()
        {
            clock.timeScale = 1;
            Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
            foreach (var tl in Utilities.FindAll<Timeline>()) // hopefully that works?
            {
                var rb = tl.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    // prevents objects from jumping, but freezes the fuck out of NPC's. this is janky but prevents npc freeze
                    if (rb.GetComponent<StressLevelZero.AI.AIBrain>() ?? rb.GetComponentInParent<StressLevelZero.AI.AIBrain>() == null) rb.Sleep();
                }

                GameObject.Destroy(tl);
            }
            isBTBAlreadyActive = false;
        }

        //object sidtoken;
        private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
        {
            //if (sidtoken != null) MelonCoroutines.Stop(sidtoken);
            //sidtoken = MelonCoroutines.Start(SlowItDown());

            var bullet = ProjectilePool._instance.lastSpawn.gameObject;
            Chaos.Log("Got the bullet");
            
            if (bullet.GetComponent<SphereCollider>() == null)
            {
                var col = bullet.AddComponent<SphereCollider>();
                col.radius = 2;
                clock._collider = col;
            }

            //var proj = bullet.GetComponent<Projectile>();
            //var rb = proj.rb;
            //rb.velocity = rb.velocity.normalized * 100;
            //Chaos.Log("Set the velocity");

            //if (bullet.GetComponent<AreaClock3D>() == null)
            //{
            //    var col = bullet.AddComponent<SphereCollider>();
            //    col.radius = 4;
            //    col.isTrigger = true;
            //    Chaos.Log("Added the collider");

            //    var ac = bullet.AddComponent<AreaClock3D>();
            //    ac._collider = col;
            //    Chaos.Log("added the clock");
            //    ac.innerBlend = ClockBlend.Multiplicative;
            //    Chaos.Log("set the blend mode");
            //    ac.timeScale = 0.01f;
            //    Chaos.Log("set the timescale");
            //}
        }

        private IEnumerator KeepCenter(GameObject bullet, AreaClock3D clock)
        {

            while (bullet != null && bullet.active)
            {
                clock.center = bullet.transform.position;
                yield return null;
            }
        }

        private IEnumerator SlowItDown()
        {
            var tuple = (0.125f, 1f);
            for (float i = 0; tuple.Slerp(i) < 1; i += Time.deltaTime)
            {
                Time.timeScale = tuple.Slerp(i);
                yield return null;
            }

        }
    }
}
