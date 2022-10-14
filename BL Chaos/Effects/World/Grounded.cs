using UnityEngine;

namespace BLChaos.Effects;

internal class Grounded : EffectBase
{
    public Grounded() : base("Grounded", 60) { }

    public override void OnEffectUpdate() => GlobalVariables.Player_PhysBody.AddImpulseForce(Vector3.down * Time.deltaTime * 2000);

}
