﻿using BoneLib;
using Jevil;
using SLZ.Props.Weapons;

namespace BLChaos.Effects;

internal class BarrySteakfries : EffectBase
{
    public BarrySteakfries() : base("Barry Steakfries", 60) { }
    [RangePreference(0.25f, 50, 0.25f)] static readonly float forceMultiplier = 1;

    public override void OnEffectStart()
    {
        Hooking.OnPostFireGun += OnFire;
    }

    public override void OnEffectEnd()
    {
        Hooking.OnPostFireGun -= OnFire;
    }

    private void OnFire(Gun gun)
    {
        GlobalVariables.Player_PhysRig.AddVelocityChange(5 * forceMultiplier * -gun.transform.forward);
    }

}
