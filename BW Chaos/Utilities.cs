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
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            ad.transform.position = phead.position + phead.forward.normalized * 2;
            ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.position);
            return ad;
        }

        public static IEnumerable<T> FindAll<T>() where T : UnityEngine.Object
        {
            return GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.Of<T>()).Select(obj => obj.Cast<T>());
        }
    }
}
