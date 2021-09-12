using System;
using UnityEngine;
using MelonLoader;
using HarmonyLib;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;

/* 
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 * I TOOK IT FROM GITHUB AND CHANGED IT A LITTLE
 *      https://github.com/Evanaellio/HyperJump/blob/master/HyperJump/HyperJump.cs
 * THIS REPO HAS THE MIT LICENSE SO ITS FINE, I THINK. EVEN THOUGH BWCHAOS IS GPLV3.
 *      IF ANY LAWYERS WANT TO SUE ME OVER IT, PLEASE DONT
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 */

namespace BWChaos.Effects
{
    internal class HighJump : EffectBase
    {
        public HighJump() : base("High jump") { }

        public static bool isJumpEnabled = false;
        public static float FwJumpMult = 5f;
        public static float UpJumpMult = 20f;
        public override void OnEffectStart() => isJumpEnabled = true;
        public override void OnEffectEnd() => isJumpEnabled = false;


    }

    [HarmonyPatch(typeof(ControllerRig), "Jump")]
    class ControllerRigJumpPatch
    {
        public static void Prefix(ControllerRig __instance)
        {
            if (!HighJump.isJumpEnabled) return;

            var physGrounder = GameObject.FindObjectOfType<PhysGrounder>();

            // Only jump when on the ground
            if (physGrounder.isGrounded)
            {
                PhysicsRig rig = GameObject.FindObjectOfType<PhysicsRig>();

                // Compute velocity vectors for jumping (up) and leaping (forward)
                float walkSpeed = new Vector3(rig.pelvisVelocity.x, 0, rig.pelvisVelocity.z).magnitude;
                Vector3 forwardJump = __instance.m_head.forward * walkSpeed * HighJump.FwJumpMult;
                Vector3 verticalJump = Vector3.up * HighJump.UpJumpMult;

                // Apply jump velocity to the player
                rig.physBody.AddVelocityChange(verticalJump + forwardJump);
            }
        }
    }
}

/* 
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 * I TOOK IT FROM GITHUB AND CHANGED IT A LITTLE
 *      https://github.com/Evanaellio/HyperJump/blob/master/HyperJump/HyperJump.cs
 * THIS REPO HAS THE MIT LICENSE SO ITS FINE, I THINK. EVEN THOUGH BWCHAOS IS GPLV3.
 *      IF ANY LAWYERS WANT TO SUE ME OVER IT, PLEASE DONT
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 */