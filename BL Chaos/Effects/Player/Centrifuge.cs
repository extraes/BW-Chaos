using Jevil;
using System;
using UnityEngine;

namespace BLChaos.Effects;

internal class Centrifuge : EffectBase
{
    public Centrifuge() : base("Centrifuge", 15) { }
    [RangePreference(0, 5, 0.125f)] static readonly float forceMultiplier = 1;

    public override void OnEffectUpdate()
    {
        float r = 1;
        float theta = Time.realtimeSinceStartup % 5 * 360;
        float x = r * (float)Math.Cos(theta * Math.PI / 180);
        float y = r * (float)Math.Sin(theta * Math.PI / 180);
        GlobalVariables.Player_PhysRig.AddVelocityChange(0.25f * forceMultiplier * new Vector3(x, 0, y));
    }
}
