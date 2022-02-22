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
    internal class LockRigidBodies : EffectBase
    {
        public LockRigidBodies() : base("Lock Random Items") { }
        Dictionary<Rigidbody, RigidbodyConstraints> dict = new Dictionary<Rigidbody,RigidbodyConstraints>();

        public override void HandleNetworkMessage(string data) => Chaos.Log("I got some data! " + data);
        public override void OnEffectEnd() => Chaos.Log("Placeholder end");

        public IEnumerator CoRun()
        {
            if (isNetworked) yield break;
            yield return null;
            foreach(var rb in Utilities.FindAll<Rigidbody>())
            {
                // this is fucking horrendously bad code lol
                bool xPos = Random.value > 0.5f;
                bool yPos = Random.value > 0.5f;
                bool zPos = Random.value > 0.5f;
                bool xRot = Random.value > 0.5f;
                bool yRot = Random.value > 0.5f;
                bool zRot = Random.value > 0.5f;

                if (xPos) rb.constraints |= RigidbodyConstraints.FreezePositionX;
                if (yPos) rb.constraints |= RigidbodyConstraints.FreezePositionY;
                if (zPos) rb.constraints |= RigidbodyConstraints.FreezePositionZ;
                if (xRot) rb.constraints |= RigidbodyConstraints.FreezeRotationX;
                if (yRot) rb.constraints |= RigidbodyConstraints.FreezeRotationY;
                if (zRot) rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}
