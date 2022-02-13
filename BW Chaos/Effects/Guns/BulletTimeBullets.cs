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
using HarmonyLib;

namespace BWChaos.Effects
{
    //[DontRegisterEffect] // shits broken rn. idk how to use chronos in local mode at runtime
    internal class BulletTimeBullets : EffectBase
    {
        // as with inaccurateguns, this will fuck shit up due to latency, so dont sync
        public BulletTimeBullets() : base("Bullet Time Bullets", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
        //private static AreaClock3D clock = null;
        private static bool isBTBAlreadyActive = false; // We don't want two BTBs at the same time. That would be bad.
        static ProjectilePool projPool;
        [RangePreference(0.25f, 10f, 0.25f)] static float clockRadius = 5;
        [RangePreference(0f, 1f, 0.0025f)] static float clockTimeScale = 0.01f;
        [EffectPreference] static ClockBlend clockBlend = ClockBlend.Multiplicative;

        // I MISS THE RAGE !?!?!?!?!?!?!?!? (i thought this was funny.)
        public override void HandleNetworkMessage(string data) => "I MISS THE RAGE⁉️⁉️⁉️⁉️⁉️".Trim();
        public override void OnEffectStart()
        {
            // do the same Chronos setup shit as in Effects/Misc/Rewind.cs
            if (isBTBAlreadyActive)
            {
                Chaos.Warn("Two instances of BTB can't be active at the same time! Not running again!");
                return;
            }

            isBTBAlreadyActive = true;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
            projPool = projPool ?? GameObject.FindObjectOfType<ProjectilePool>();

            #region Set up Chronos & clock

            GameObject chronos = GameObject.Find("ChronosController") ?? new GameObject("ChronosController");
            //if (clock == null) clock = chronos.AddComponent<AreaClock3D>();

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

            Hooking.OnPostFireGun += Hooking_OnPostFireGun;

            try { while (GlobalVariables.ActiveEffects.Any(e => e.Name == "4x speed")) GlobalVariables.ActiveEffects.FirstOrDefault(e => e.Name == "4x speed")?.ForceEnd(); } catch { }
        }


        public override void OnEffectEnd()
        {
            Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
        }

        private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
        {
            var ls = projPool.lastSpawn;
            ls.currentSpeed = 5;
            ls.allowBulletDrop = false;

            var _ac = ls.GetComponent<AreaClock3D>();
            _ac?.CacheComponents();
            if (_ac != null) return; // ignore if already set up

            var col = ls.gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = clockRadius;

            var ac3d = ls.gameObject.AddComponent<AreaClock3D>();
            ac3d._collider = col;
            ac3d.timeScale = clockTimeScale;
            ac3d.innerBlend = clockBlend;
            ac3d.CacheComponents();
            ac3d._collider = col;
            
            ac3d.OnTriggerEnter(col);

            //var tl = ls.gameObject.GetComponent<Timeline>();
            //tl._clock = ;
#if DEBUG
            Chaos.Log("Set up AC3D on " + ls.transform.GetFullPath());
#endif
        }


        [HarmonyPatch(typeof(AreaClock3D), nameof(AreaClock3D.OnTriggerEnter))]
        public static class TriggerEnterPatch
        {
            public static void Prefix(AreaClock3D __instance, Collider other)
            {
                // Make sure the area clock is actually enabled
                if (!__instance.isActiveAndEnabled)
                    return;
                // Checks if the rigidbody has a timeline
                if (other.attachedRigidbody && !other.attachedRigidbody.isKinematic && !other.attachedRigidbody.gameObject.GetComponent<Timeline>())
                {
#if DEBUG
                    Chaos.Log("Rigidbody " + other.transform.GetFullPath() + " entered AC3D");
#endif
                    if (other.attachedRigidbody.transform.IsChildOfRigManager()) return;
                    Rigidbody[] rigidbodies = __instance.transform.root.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rb in rigidbodies)
                    {
                        if (rb.isKinematic) continue;

                        // If not, we create it.
                        GameObject obj = rb.gameObject;
                        // Sets up the clock
                        LocalClock clock = obj.AddComponent<LocalClock>();
                        clock.timeScale = clockTimeScale;
                        // Sets up the timeline
                        Timeline timeline = obj.AddComponent<Timeline>();
                        timeline._clock = clock;

#if DEBUG
                        Chaos.Log("Set up clock and timeline on " + other.name + " because it entered the AC3D radius");
#endif
                    }
                }
            }

            public static void Postfix(AreaClock3D __instance, Collider other)
            {
                // Make sure the area clock is actually enabled
                if (!__instance.isActiveAndEnabled)
                    return;
                // Checks if the collider has a timeline
                if (!other.gameObject.GetComponent<Timeline>() && other.attachedRigidbody && !other.attachedRigidbody.isKinematic)
                {
                    if (other.attachedRigidbody.transform.IsChildOfRigManager()) return;
                    Rigidbody[] rigidbodies = other.transform.root.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rb in rigidbodies)
                    {
                        if (rb.isKinematic) continue;

                        // If not, we capture the rigidbody
                        Timeline rbTimeline = rb.gameObject.GetComponent<Timeline>();
                        if (rbTimeline)
                        {
                            // Store local coordinates to account for dynamic changes of the clock's transform
                            Transform transform = __instance.transform;
                            Vector3 entry = transform.InverseTransformPoint(rb.worldCenterOfMass);
                            __instance.Capture(rbTimeline, entry);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(AreaClock3D), nameof(AreaClock3D.OnTriggerExit))]
        public static class TriggerExitPatches
        {
            public static void Postfix(AreaClock3D __instance, Collider collider)
            {
                if (!collider.attachedRigidbody) return;

                Rigidbody[] rigidbodies = collider.transform.root.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rb in rigidbodies)
                {
                    // Force release the rigidbody for objects with child colliders
                    Timeline rbTimeline = rb.gameObject.GetComponent<Timeline>();
                    if (rbTimeline)
                    {
                        try
                        {
                            __instance.Release(rbTimeline);
                        }
                        catch { }
                    }
                }
            }

        }
    }
}