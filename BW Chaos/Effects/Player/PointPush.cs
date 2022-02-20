using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class PointPush : EffectBase
    {
        public PointPush() : base("Point Push", 30) { }
        [RangePreference(0,10, 0.125f)] static float forceMultiplier = 1;

        public override void OnEffectUpdate()
            => GlobalVariables.Player_PhysBody.AddImpulseForce(Player.rightHand.transform.forward.normalized * 1500f * forceMultiplier * Time.deltaTime); 
        // unscaled 250f is too much when this effect runs once a frame
    }
}
