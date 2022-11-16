using Jevil;
using UnityEngine;

namespace BLChaos.Effects;

internal class Grounded : EffectBase
{
    public Grounded() : base("Grounded", 60) { }

    public override void OnEffectUpdate() => GlobalVariables.Player_PhysRig.AddVelocityChange(25 * Time.deltaTime * Vector3.down);

}
