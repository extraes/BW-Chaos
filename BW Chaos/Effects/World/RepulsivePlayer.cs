using MelonLoader;
using StressLevelZero.VRMK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class RepulsivePlayer : EffectBase
    {
        public RepulsivePlayer() : base("Repulsive player", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }

        private List<RepulseBehaviour> gameObjects = new List<RepulseBehaviour>();
        public override void OnEffectStart()
        {
            MelonCoroutines.Start(ApplyMonoBehaviour());
        }
        public override void OnEffectEnd()
        {
            foreach (var comp in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<RepulseBehaviour>())) GameObject.Destroy(comp);
        }

        private IEnumerator ApplyMonoBehaviour()
        {
            bool stagger = false;
            foreach (var rb in GameObject.FindObjectsOfType<Rigidbody>())
            {
                // we dont want to mess with things that already have joints, are in the list, or are static
                var go = rb.gameObject; //                V luckily passing null to contains doesnt error out
                if (gameObjects.Contains(go.GetComponent<RepulseBehaviour>())) continue;
#if DEBUG
                //MelonLogger.Msg($"Gave {go.name} the script");
#endif

                gameObjects.Add(go.AddComponent<RepulseBehaviour>());

                if (stagger = !stagger) yield return new WaitForFixedUpdate();

            }

        }
    }

    [RegisterTypeInIl2Cpp]
    public class RepulseBehaviour : MonoBehaviour
    {
        public RepulseBehaviour(IntPtr ptr) : base(ptr) { }

        private static readonly float mult = 0.5f;
        //                         optuhmuhzayshun V
        private static readonly int framesToWait = 4;
        private static Transform target;
        private bool isNear = false;
        private Rigidbody rb;
        private object CToken;
        public void OnEnable()
        {
            target = GlobalVariables.Player_PhysBody.transform;
            rb = GetComponent<Rigidbody>();
            CToken = MelonCoroutines.Start(CheckDist());
        }

        // shoutouts to camobiwon for suggesting i use a pd controller (and sending link)
        public void FixedUpdate()
        {
            if (!isNear || (Time.frameCount % framesToWait != 0)) return;

            // https://digitalopus.ca/site/pd-controllers/ lol
            float dt = Time.fixedDeltaTime;
            Vector3 p = transform.position; //our current position
            Vector3 v = rb.velocity; //our current velocity
            // subt V3.p because then rb's wont try to go into the floor    V
            Vector3 force = rb.mass * (target.transform.position - Vector3.up - p - v * dt) / (dt);

            rb.AddForce(-Vector3.ClampMagnitude(force * mult, 100 * rb.mass));
        }

        public void Destroy ()
        {
            MelonCoroutines.Stop(CToken);
        }

        private IEnumerator CheckDist()
        {
            while (true)
            {
                if (this?.gameObject == null || !gameObject.active) yield break;
                isNear = ((target.position - gameObject.transform.position).sqrMagnitude < 15 * 15) && transform.root.name != "[RigManager (Default Brett)]";
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }

}
