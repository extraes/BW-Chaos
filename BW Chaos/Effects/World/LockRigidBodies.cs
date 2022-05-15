using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects;

internal class LockRigidBodies : EffectBase
{
    public LockRigidBodies() : base("Lock Random Items", 15) { }

    readonly Dictionary<Rigidbody, RigidbodyConstraints> dict = new Dictionary<Rigidbody, RigidbodyConstraints>();
    [RangePreference(1, 150, 2)] static readonly float rbsPerFrame = 10;

    public override void OnEffectEnd()
    {
        foreach (KeyValuePair<Rigidbody, RigidbodyConstraints> pair in dict)
        {
            pair.Key.constraints = pair.Value;
        }
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        string path = Encoding.ASCII.GetString(data[0]);
        Rigidbody rb = GameObject.Find(path)?.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Chaos.Warn("Rigidbody not found in client! -> " + path);
        }
        dict.Add(rb, rb.constraints);
        RigidbodyConstraints constraints = (RigidbodyConstraints)BitConverter.ToInt32(data[1], 0);
        rb.constraints = constraints;
    }

    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        yield return null;
        int counter = 0;
        foreach (Rigidbody rb in Utilities.FindAll<Rigidbody>())
        {
            dict.Add(rb, rb.constraints);
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
            SendNetworkData(Encoding.ASCII.GetBytes(rb.transform.GetFullPath()), BitConverter.GetBytes((int)rb.constraints));
            if (counter++ % rbsPerFrame == 0) yield return null;
        }
    }
}
