using HarmonyLib;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using StressLevelZero.VRMK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BWChaos;

public static class Utilities
{
    static readonly HarmonyMethod skipMethod = new HarmonyMethod(typeof(Utilities), nameof(Utilities.SkipMethod));
    static readonly MethodInfo slowMoMethod = typeof(Control_GlobalTime).GetMethod(nameof(Control_GlobalTime.DECREASE_TIMESCALE));
    static bool skipSlow = false;
    public static GameObject GetPrefabOfPool(string objectName)
    {
        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, Pool> dynamicPool in PoolManager.DynamicPools)
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
        GameObject ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(str);
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

    [Obsolete("Stringing Vector3's is data inefficient - use the byte utilties like " + nameof(DebyteV3) + " and the extension " + nameof(Extensions.ToBytes) + " instead")]
    public static Vector3 DestringV3(string vecStr, char delimiter = ',')
    {
        string[] vs = vecStr.Split(delimiter);
        float[] vs1 = vs.Select(v => float.Parse(v)).ToArray();
        return new Vector3(vs1[0], vs1[1], vs1[2]);
    }

    public static Vector3 DebyteV3(byte[] bytes, int startIdx = 0)
    {
#if DEBUG
        if (startIdx == 0 && bytes.Length != Const.SizeV3) Chaos.Warn($"Trying to debyte a Vector3 of length {bytes.Length}, this is not the expected {Const.SizeV3} bytes!");
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
            byte[] _vecB = vectors[i].ToBytes();
            Buffer.BlockCopy(_vecB, 0, bytes, offset, sizeof(float) * 3);
        }
        return bytes;
    }


    public static void MoveAndFacePlayer(GameObject obj)
    {
        Transform phead = GlobalVariables.Player_PhysBody.rbHead.transform;
        obj.transform.position = phead.position + phead.forward.normalized * 2;
        obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - phead.position, Vector3.up);
    }

    /* Input: [
     *      [1,2,3,4,5,6,7]
     *      [2,3,4,5,6,7,8,9,10]
     * ]
     * Output: [
     *      3, <- header, says how many things there are
     *      ushorts(7,9)  <- header, says where to split
     *      1,2,3,4,5,6,7,
     *      2,3,4,5,6,7,8,9,10
     *      
     * ]
     */
    public static byte[] JoinBytes(byte[][] bytess)
    {
        byte arrayCount = (byte)bytess.Length;
        // bytes.Length - 1 because you dont put a delim at the end (unless youre weird i guess, but im not)
        List<ushort> indices = new List<ushort>(bytess.Length - 1);
        byte[] bytesJoined = new byte[(bytess.Length * 2 + 1) + bytess.Sum(b => b.Length)];
        // need to get the indices beforehand to compose the header
        foreach (byte[] arr in bytess)
        {
            indices.Add((ushort)arr.Length);
        }
        // Compose the header
        bytesJoined[0] = arrayCount;
        for (int i = 0; i < indices.Count; i++)
        {
            ushort idx = indices[i];
            BitConverter.GetBytes(idx).CopyTo(bytesJoined, i * sizeof(ushort) + 1);
        }

        ushort lastLen = 0;
        int index = arrayCount * 2 + 1;
        for (int i = 0; i < arrayCount; i++)
        {
            int thisLen = indices[i];
            Buffer.BlockCopy(bytess[i], 0, bytesJoined, index, thisLen);

            // set the next index
            lastLen = (ushort)thisLen;
            index += thisLen;
        }

        return bytesJoined;
    }

    internal static byte[][] SplitBytes(byte[] bytes)
    {
        // Grab data from the header
        // get number of splits
        int splitsCount = bytes[0];
        // actually get the list of element sizes
        int[] splitIndices = new int[splitsCount];
        for (int i = 0; i < splitsCount; i++)
        {
            // add 1 to skip the byte that says how many split indicies there are
            ushort idx = BitConverter.ToUInt16(bytes, 1 + i * sizeof(ushort));
            splitIndices[i] = idx;
        }

        byte[][] res = new byte[splitsCount][];
        // start after the header
        int lastIdx = splitsCount * sizeof(ushort) + 1;
        for (int i = 0; i < res.Length; i++)
        {
            int len = splitIndices[i];
            // dont overallocate lest there be a dead byte
            byte[] arr = new byte[len];
            // copy the bytes
            Buffer.BlockCopy(bytes, lastIdx, arr, 0, len);


            // save our changes to the array 
            res[i] = arr;
            // perpetuate the nevereding cycle
            lastIdx += len;
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

    public static void MultiplyForces(PhysHand hand, float mult = 200f, float torque = 10f)
    {

        /*
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         * I GANKED THE FUCK OUT OF THIS CODE FROM LAKATRAZZ'S SPIDERMAN MOD
         *      IT'S NOT OPEN SOURCE SO I HOPE I DON'T GET SUED, BUT IF HE WANTS THIS GONE, FINE BY ME, ILL REMOVE IT
         *      I DNSPY'D THE SPIDERMAN MOD AND COPY PASTED THE SPIDERMAN.MODOPTIONS.MULTIPLYFORCES METHOD
         *      I THINK ITS FINE THO CAUSE I ASKED HIM AND HE SAID "just take code from any of my mods tbh i dont care"
         *      (https://discord.com/channels/563139253542846474/724595991675797554/913613134885974036)
         * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
         */

        hand.xPosForce = 90f * mult;
        hand.yPosForce = 90f * mult;
        hand.zPosForce = 340f * mult;
        hand.xNegForce = 90f * mult;
        hand.yNegForce = 200f * mult;
        hand.zNegForce = 360f * mult;
        hand.newtonDamp = 80f * mult;
        hand.angDampening = torque;
        hand.dampening = 0.2f * mult;
        hand.maxTorque = 30f * torque;
    }
}
