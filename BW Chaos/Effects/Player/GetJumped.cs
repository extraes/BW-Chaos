using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using StressLevelZero.Pool;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class GetJumped : EffectBase
    {
        public GetJumped() : base("Get Jumped") { }

        public override void OnEffectStart()
        {
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            for (int i = 0; i < 8; i++)
            {
                float theta = (i / 8) * 360;
                float x = (float)(Math.Cos(theta * Math.PI / 180));
                float y = (float)(Math.Sin(theta * Math.PI / 180));

                Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
                CustomItems.SpawnFromPool("Null Body", spawnPos, Quaternion.identity);
            }
        }
    }
}
