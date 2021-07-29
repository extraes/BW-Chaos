using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using StressLevelZero.VRMK;

namespace BWChaos.Effects
{
    internal class RepulsivePlayer : EffectBase
    {
        public RepulsivePlayer() : base("Repulsive player", 60) { }


        private List<Rigidbody> rigidbodies = new List<Rigidbody> { };
        private object[] coroutineTokens = new object[2];
        public override void OnEffectStart()
        {
            coroutineTokens[0] = MelonCoroutines.Start(refreshRigidbodies());
            coroutineTokens[1] = MelonCoroutines.Start(ApplyForces());
        }
        public override void OnEffectEnd()
        {
            MelonCoroutines.Stop(coroutineTokens[0]);
            MelonCoroutines.Stop(coroutineTokens[1]);
        }

        private IEnumerator ApplyForces()
        {
            yield return new WaitForFixedUpdate();
            if (!Active) yield break;
            Vector3 pos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;

            if (!Active)
            {
                var rbArray = rigidbodies.ToArray();
                for (int i = 0; i< rbArray.Length; i++) {
                    var rb = rbArray[i];
                    if (!Active || rb == null) yield break;
                    if (i % 5 == 0) pos = GameObject.FindObjectOfType<PhysBody>().rbHead.transform.position;
                    rb.AddExplosionForce(3f, pos, 10, 2, ForceMode.VelocityChange);
                    if (i % 2 == 0) yield return new WaitForFixedUpdate();
                }
            }
            MelonCoroutines.Start(ApplyForces());
        }

        private IEnumerator refreshRigidbodies()
        {
            if (!Active) yield break;

            // Don't modify the rigidbody list without marking it
            rigidbodies.Clear();
            // Get the position of the player's head
            var physBody = GameObject.FindObjectOfType<PhysBody>();
            if (physBody == null) yield break;
            var pos = physBody.rbHead.transform.position;

            // For every collider in 10 meters, make sure it isn't already in the list and make sure it's not a part of the player.
            var cols = Physics.OverlapSphere(pos, 10);
            for (int i = 0; i< cols.Length; i++)
            {
                Collider col = cols[i];
                if (!Active || col == null) yield break;
                if (!(rigidbodies.Contains(col.attachedRigidbody) || col.gameObject.transform.root.name == "[RigManager (Default Brett)]"))
                {
                    if (col.attachedRigidbody != null)
                        rigidbodies.Add(col.attachedRigidbody);
                }
                if (i % 5 == 0) yield return new WaitForFixedUpdate();

            }

            yield return new WaitForSecondsRealtime(1);
            MelonCoroutines.Start(refreshRigidbodies());
        }
    }
}
