using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class RandomMesh : EffectBase
    {
        public RandomMesh() : base("My Experience in Blender") { }
        [RangePreference(0.02f, 2, 0.02f)] static float scaleMultiplier = 0.5f;

        public override void HandleNetworkMessage(byte[] data) 
        {

            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var mesh = go.GetComponent<MeshFilter>().mesh;

            mesh.vertices = Utilities.DeserializeMesh(data.Skip(2).ToArray());

            Utilities.MoveAndFacePlayer(go);
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;
            var col = go.AddComponent<MeshCollider>();
            col.convex = true;

            go.transform.DeserializePosRot(data);
        }

        public override void OnEffectStart()
        {
            if (isNetworked) return;

            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            var mesh = go.GetComponent<MeshFilter>().mesh;

            int vertCount = mesh.vertices.Length;
#if DEBUG
            Chaos.Log("Created GameObject " + go.name + " has " + vertCount + " vertices");
#endif
            mesh.vertices = GenerateMesh(vertCount);
            Utilities.MoveAndFacePlayer(go);
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;
            var col = go.AddComponent<MeshCollider>();
            col.convex = true;

            byte[][] bytes = new byte[][]
            {
                go.transform.SerializePosRot(),
                Utilities.SerializeMesh(mesh.vertices)
            };


            SendNetworkData(bytes.Flatten());
        }

        private Vector3[] GenerateMesh(int vertices)
        {
            Vector3 runningTotal = new Vector3();
            Vector3[] verts = new Vector3[vertices];
            for (int i = 0; i < vertices; i++)
            {
                verts[i] = (runningTotal += Random.insideUnitSphere);
                if (i % 3 == 0) runningTotal = runningTotal.normalized * runningTotal.magnitude * scaleMultiplier;
            }
            return verts;
        }
    }
}
