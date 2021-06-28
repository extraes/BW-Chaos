using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;

namespace BW_Chaos.Effects
{
    internal class PointPush : EffectBase
    {
        public PointPush() : base("Point Push", 30) { }

        public override void OnEffectUpdate()
            => GlobalVariables.Player_PhysBody.AddImpulseForce(Player.rightHand.transform.forward.normalized * 1000f);
    }
}
