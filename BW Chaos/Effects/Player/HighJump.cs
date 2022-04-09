using HarmonyLib;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using System;
using UnityEngine;

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
        public HighJump() : base("High jump", 90) { }

        [RangePreference(0, 25, 1)] public static float forwardJumpMult = 5f;
        [RangePreference(0, 50, 2)] public static float upJumpMult = 20f;
        public static Action OnJump;
        public override void OnEffectStart() => OnJump += HiJump;
        public override void OnEffectEnd() => OnJump -= HiJump;

        private void HiJump ()
        {
            if (!Active) return;
            PhysicsRig rig = GlobalVariables.Player_RigManager.physicsRig;

            // Compute velocity vectors for jumping (up) and leaping (forward)
            float walkSpeed = new Vector3(rig.pelvisVelocity.x, 0, rig.pelvisVelocity.z).magnitude;
            Vector3 forwardJump = GlobalVariables.Player_RigManager.ControllerRig.m_head.forward * walkSpeed * forwardJumpMult;
            Vector3 verticalJump = Vector3.up * HighJump.upJumpMult;

            // Apply jump velocity to the player
            rig.physBody.AddVelocityChange(verticalJump + forwardJump);
        }


        [HarmonyPatch(typeof(ControllerRig), "Jump")]
        public class ControllerRigJumpPatch
        {
            public static void Postfix()
            {
                var physGrounder = GameObject.FindObjectOfType<PhysGrounder>();

                // Only jump when on the ground
                if (physGrounder.isGrounded)
                {
                    OnJump?.Invoke();
                }
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