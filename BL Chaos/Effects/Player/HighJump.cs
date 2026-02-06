/* 
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 * I TOOK IT FROM GITHUB AND CHANGED IT A LITTLE
 *      https://github.com/Evanaellio/HyperJump/blob/master/HyperJump/HyperJump.cs
 * THIS REPO HAS THE MIT LICENSE SO ITS FINE, I THINK. EVEN THOUGH BWCHAOS IS GPLV3.
 *      IF ANY LAWYERS WANT TO SUE ME OVER IT, PLEASE DONT
 * THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE - THIS CODE IS NOT MINE
 */

using Jevil.Patching;

namespace BLChaos.Effects;

internal class HighJump : EffectBase
{
    public HighJump() : base("High jump", 90) { }

    [RangePreference(0, 25, 1)] public static float forwardJumpMult = 5f;
    [RangePreference(0, 50, 2)] public static float upJumpMult = 20f;
    public static Action? onJump;
    public override void OnEffectStart() => GameCallbacks.OnJump += HiJump;
    public override void OnEffectEnd() => GameCallbacks.OnJump -= HiJump;

    private void HiJump()
    {
        if (!Active) return;
        PhysicsRig rig = GlobalVariables.Player_RigManager.physicsRig;

        // Compute velocity vectors for jumping (up) and leaping (forward)
        float walkSpeed = new Vector3(rig.pelvisVelocity.x, 0, rig.pelvisVelocity.z).magnitude;
        Vector3 forwardJump = GlobalVariables.Player_RigManager.ControllerRig.m_head.forward * walkSpeed * forwardJumpMult;
        Vector3 verticalJump = Vector3.up * HighJump.upJumpMult;

        // Apply jump velocity to the player
        rig.AddVelocityChange(verticalJump + forwardJump);
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