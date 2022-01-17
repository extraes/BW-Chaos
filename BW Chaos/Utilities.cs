using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace BWChaos
{
    public static class Utilities
    {


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

        public static Vector3 DebyteV3(byte[] bytes)
        {
#if DEBUG
            if (bytes.Length != sizeof(float) * 3) Chaos.Warn($"Trying to debyte a Vector3 of length {bytes.Length}, this is not the expected {sizeof(float) * 3} bytes!");
#endif
            return new Vector3(
                BitConverter.ToSingle(bytes, 0), 
                BitConverter.ToSingle(bytes, sizeof(float)), 
                BitConverter.ToSingle(bytes, sizeof(float) * 2));
        }

        public static void MoveAndFacePlayer(GameObject obj)
        {
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            obj.transform.position = phead.position + phead.forward.normalized * 2;
            obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - phead.position, Vector3.up);
        }

        public static byte[] JoinBytes(byte[][] bytess, byte delim = 255)
        {
            byte[] bytesJoined = new byte[(bytess.Length - 1) + bytess.Sum(b => b.Length)];

            int i = 0, offset = 0;
            while (i < bytess.Length)
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
                i++;
            }

            return bytesJoined;
        }

        public static byte[][] SplitBytes(byte[] bytes, byte delim = 255)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            List<List<byte>> bytess = new List<List<byte>>() { new List<byte>(), };

            #region Do the actual splitting

            int idx = 0;
            foreach (var b in bytes)
            {
                if (b == delim)
                {
                    idx++;
                    bytess.Add(new List<byte>());
                }
                else
                {
                    bytess[idx].Add(b);
                }
            }

            #endregion

            #region Convert to byte[][]

            List<byte[]> bytessListicle = new List<byte[]>();
            foreach (var b in bytess)
            {
                bytessListicle.Add(b.ToArray());
            }

            #endregion
#if DEBUG
            sw.Stop();
            Console.WriteLine($"Split array of {bytes.Length} into array array of {bytessListicle.Count} length in {sw.ElapsedMilliseconds}ms");
#endif
            return bytessListicle.ToArray();
        }
    }
}
