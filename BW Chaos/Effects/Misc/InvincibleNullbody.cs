using System;
using UnityEngine;
using MelonLoader;
using StressLevelZero.Pool;
using System.Linq;
using StressLevelZero.AI;

namespace BWChaos.Effects
{
    internal class PunchingBagNullbody : EffectBase
    {
        public PunchingBagNullbody() : base("Spawn invincible nullbody") { }

        public override void OnEffectStart()
        {
            Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
            if (pool == null)
            {
                Chaos.Warn("Nullbody pool not found! Why?");
                return;
            }
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            Vector3 spawnPos = playerPos + GlobalVariables.Player_PhysBody.rbHead.transform.forward;
            var spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            
            var nullbody = pool.InstantiatePoolee(playerPos, spawnRot);
            
            nullbody.gameObject.SetActive(true);
            // im not sure _why_ kinematic punching-bag-ify's it, but im not complaining 
            nullbody.StartCoroutine(nullbody.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive());
            nullbody.GetComponent<AIBrain>().behaviour.health.cur_hp = int.MaxValue;

        }
    }
}
