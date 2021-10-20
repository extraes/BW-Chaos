using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class PointPush : EffectBase
    {
        public PointPush() : base("Point Push", 30) { }

        public override void OnEffectUpdate()
            => GlobalVariables.Player_PhysBody.AddImpulseForce(Player.rightHand.transform.forward.normalized * 250f * Time.deltaTime); // 250f is too much when this effect runs once a frame
    }
}
