using HarmonyLib;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BWChaos
{
    public static class Utilities
    {
        static readonly HarmonyMethod skipMethod = new HarmonyMethod(typeof(Utilities), nameof(Utilities.SkipMethod));
        static readonly MethodInfo slowMoMethod = typeof(Control_GlobalTime).GetMethod(nameof(Control_GlobalTime.DECREASE_TIMESCALE));
        static bool skipSlow = false;
        public static GameObject GetPrefabOfPool(string objectName)
        {
            foreach (var dynamicPool in PoolManager.DynamicPools)
            {
                if (dynamicPool.Key == objectName)
                    return dynamicPool.Value.Prefab;
            }

            return null;
        }

        public static Hand GetRandomPlayerHand()
        {
            int randomNum = UnityEngine.Random.Range(0, 2);
            if (randomNum == 1)
                return Player.leftHand;
            else
                return Player.rightHand;
        }

        public static GameObject SpawnAd(string str)
        {
            var ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(str);
            MoveAndFacePlayer(ad);
            return ad;
        }

        public static IEnumerable<T> FindAll<T>() where T : UnityEngine.Object
        {
            return GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<T>()).Select(obj => obj.Cast<T>());
        }

        public static string[] Argsify(string data, char split)
        {
            string[] args = data.Split(split);
            string arg = args[0];
            string argg = string.Join(split.ToString(), args.Skip(1));
            return new string[] { arg, argg };
        }

        public static Vector3 DestringV3(string vecStr, char delimiter = ',')
        {
            string[] vs = vecStr.Split(delimiter);
            float[] vs1 = vs.Select(v => float.Parse(v)).ToArray();
            return new Vector3(vs1[0], vs1[1], vs1[2]);
        }

        public static Vector3 DebyteV3(byte[] bytes, int startIdx = 0)
        {
#if DEBUG
            if (startIdx == 0 && bytes.Length != GlobalVariables.Vector3Size) Chaos.Warn($"Trying to debyte a Vector3 of length {bytes.Length}, this is not the expected {sizeof(float) * 3} bytes!");
            if (startIdx + (sizeof(float) * 3) > bytes.Length) Chaos.Warn($"{bytes.Length} is too short for the given index of {startIdx}");
#endif
            return new Vector3(
                BitConverter.ToSingle(bytes, startIdx),
                BitConverter.ToSingle(bytes, startIdx + sizeof(float)),
                BitConverter.ToSingle(bytes, startIdx + sizeof(float) * 2));
        }

        public static Vector3[] DeserializeMesh(byte[] bytes)
        {
            int vecSize = (sizeof(float) * 3);
            Vector3[] result = new Vector3[bytes.Length / vecSize];
#if DEBUG
            if (bytes.Length % vecSize != 0) Chaos.Warn("Malformed byte array - not divisible by the size of a vector3");
#endif
            for (int i = 0; i < bytes.Length; i += vecSize)
            {
                result[i / vecSize] = DebyteV3(bytes, i);
            }

            return result;
        }

        public static byte[] SerializeMesh(Vector3[] vectors)
        {
            byte[] bytes = new byte[vectors.Length * sizeof(float) * 3];
            for (int i = 0; i < vectors.Length; i++)
            {
                int offset = i * sizeof(float) * 3;
                var _vecB = vectors[i].ToBytes();
                Buffer.BlockCopy(_vecB, 0, bytes, offset, sizeof(float) * 3);
            }
            return bytes;
        }


        public static void MoveAndFacePlayer(GameObject obj)
        {
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            obj.transform.position = phead.position + phead.forward.normalized * 2;
            obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - phead.position, Vector3.up);
        }

        public static byte[] JoinBytes(byte[][] bytess, byte delim = 255)
        {
            //todo: make this declare the cut indices at the start of the array
            // bytes.Length - 1 because you dont put a delim at the end (unless youre weird i guess)
            byte[] bytesJoined = new byte[(bytess.Length - 1) + bytess.Sum(b => b.Length)];

            int offset = 0;
            for (int i = 0; i < bytess.Length; i++)
            {
                Buffer.BlockCopy(bytess[i], 0, bytesJoined, offset, bytess[i].Length);
                
#if DEBUG
                for (int dT = offset; dT < bytess[i].Length + offset; dT++)
                {
                    if (bytesJoined[dT] == delim)
                        Chaos.Error($"Byte in bytesJoined at index {dT} (from start pos {i} to end pos {bytess[i].Length + offset}) is the same as the deilimiter byte ({delim})!!! This is bad!!!!!");
                }
#endif

                if (offset + bytess[i].Length < bytesJoined.Length)
                {
                    bytesJoined[offset + bytess[i].Length] = delim;
                }
                offset += bytess[i].Length + 1;
            }

            return bytesJoined;
        }

        public static byte[][] SplitBytes(byte[] bytes, byte delim = 255)
        {
            List<int> splitIndices = new List<int>();

            // loop through the array to find the delim bytes
            for (int i = 0; i < bytes.Length; i++)
            {
                // add (i + 1) because thats where the next array should start, not on the delim
                if (bytes[i] == delim) splitIndices.Add(i + 1);
            }
            // add one last one idx otherwise itll think the end is the second to last segment
            splitIndices.Add(bytes.Length + 1);

            // dont overallocate *this* array, and avoid using List cause arrays feel smarter
            byte[][] res = new byte[splitIndices.Count][];
            // start at zero
            int lastIdx = 0;
            for (int i = 0; i < res.Length; i++)
            {
                int thisIdx = splitIndices[i];
                // subtract one because the delim (usually 255) takes up a space
                int len = thisIdx - lastIdx - 1;
                // dont overallocate
                byte[] arr = new byte[len];
                // copy the bytes
                Buffer.BlockCopy(bytes, lastIdx, arr, 0, len);

                // save our changes to the array 
                res[i] = arr;
                // perpetuate the nevereding cycle
                lastIdx = splitIndices[i];
            }
            return res;
        }

        public static string GetReadableStringFromMemberName(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length);
            builder.Append(char.ToUpper(name[0]));

            for (int i = 1; i < name.Length; i++)
            {
                // use char's cause theyre on the stack not heap, and i want to avoid allocating shit 
                char character = name[i];

                if (char.IsUpper(character))
                {
                    builder.Append(' ');
                    builder.Append(char.ToUpper(character));
                }
                else builder.Append(character);
            }
#if DEBUG
            Chaos.Log("Converted member name from " + name + " to " + builder.ToString());
#endif
            return builder.ToString();
        }

        static bool SkipMethod()
        {
#if DEBUG
            Chaos.Log("Skipped a method (probably decrease_timescale) -> returning " + !skipSlow);
#endif
            return !skipSlow;
        }
        public static void DisableSloMo()
        {
            Chaos.Instance.HarmonyInstance.Patch(slowMoMethod, prefix: skipMethod);
            skipSlow = true;
        }

        public static void EnableSloMo()
        {
            Chaos.Instance.HarmonyInstance.Unpatch(slowMoMethod, HarmonyPatchType.Prefix);
            skipSlow = false;
        }
    }
}