﻿using StressLevelZero.Pool;
using System;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class GetJumped : EffectBase
    {
        public GetJumped() : base("Get Jumped") { }

        public override void OnEffectStart()
        {
            Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
            if (pool == null) return;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;

            for (int i = 0; i < 8; i++)
            {
                float theta = (i / 8f) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                var spawnRot = Quaternion.LookRotation(playerPos - spawnPos, new Vector3(0, 1, 0));
                var spawnedNB = pool.InstantiatePoolee(spawnPos, spawnRot);
                spawnedNB.gameObject.SetActive(true);
                spawnedNB.GetComponentInChildren<PuppetMasta.PuppetMaster>().DisabledToActive();
            }
        }
    }
}