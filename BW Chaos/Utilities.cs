using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using StressLevelZero.Pool;
using StressLevelZero.Interaction;
using ModThatIsNotMod;

namespace BWChaos
{
    public static class Utilities
    {
        public static string RemoveFirstLines(this string s, int n)
        {
            string[] lines = s
                .Split(System.Environment.NewLine.ToCharArray())
                .Skip(n)
                .ToArray();

            string output = string.Join(System.Environment.NewLine, lines);
            return output;
        }

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
            int randomNum = Random.Range(0, 2);
            if (randomNum == 1)
                return Player.leftHand;
            else
                return Player.rightHand;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static GameObject SpawnAd(string str)
        {
            var ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(str);
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            ad.transform.position = phead.position + phead.forward.normalized * 2;
            ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.position);
            return ad;
        }
    }
}
