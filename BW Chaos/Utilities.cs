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

        public static Vector3 DeserializeV3(string vecStr, char delimiter = ',')
        {
            string[] vs = vecStr.Split(delimiter);
            float[] vs1 = vs.Select(v => float.Parse(v)).ToArray();
            return new Vector3(vs1[0], vs1[1], vs1[2]);
        }

        public static void MoveAndFacePlayer(GameObject obj)
        {
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            obj.transform.position = phead.position + phead.forward.normalized * 2;
            obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - phead.position, Vector3.up);
        }
    }
}
