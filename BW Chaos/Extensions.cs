using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BWChaos
{
    public static class Extensions
    {
        public static string RemoveFirstLines(this string s, int n)
        {
            string[] lines = s
                .Split(Environment.NewLine.ToCharArray())
                .Skip(n)
                .ToArray();

            string output = string.Join(System.Environment.NewLine, lines);
            return output;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        public static bool InHierarchyOf(this Transform t, string parentName)
        {
            if (t.name == parentName)
                return true;

            if (t.parent == null)
                return false;

            t = t.parent;

            return InHierarchyOf(t, parentName);
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> fun)
        {
            foreach (T item in sequence) fun(item);
        }

        public static float Slerp(this (float, float) tuple, float time)
        {

            float clampTime = Mathf.Clamp(time, 0, 1);
            float correctedCos = -(Mathf.Cos(clampTime * (float)Math.PI) - 1) / 2;
            float diff = tuple.Item2 - tuple.Item1;
            return tuple.Item1 + (diff * correctedCos);
        }

        public static void Destroy(this GameObject go) => GameObject.Destroy(go);

        public static void UseEmbeddedResource(this System.Reflection.Assembly assembly, string resourcePath, Action<byte[]> whatToDoWithResource)
        {
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                // Don't overallocate memory to the mstream (?).
                using (System.IO.MemoryStream mStream = new System.IO.MemoryStream((int)stream.Length))
                {
                    // Copy the stream to a memorystream. Why? Don't know, ask .NET 4.7.2 designers.
                    stream.CopyTo(mStream);
                    whatToDoWithResource(mStream.ToArray());
                }
            }
        }

        public static T Random<T>(this IEnumerable<T> sequence)
        {
            return sequence.ElementAt(UnityEngine.Random.Range(0, sequence.Count()));
        }

        public static string GetFullPath(this Transform t)
        {
            string path = "/" + t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = "/" + t.name + path;
            }
            return path;
        }

        public static string[] Serialize(this Vector3 vec, int decimals, char delimiter = ',')
        {
            float[] floats = new float[] { vec.x, vec.y, vec.z, };
            string[] strings = floats.Select(f => Math.Round(f, decimals).ToString()).ToArray();
            return strings;
        }

        public static string Join<T>(this IEnumerable<T> seq, string delim = ",")
        {
            return string.Join(delim, seq);
        }
    }
}
