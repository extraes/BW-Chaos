using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using SLZ.Props.Weapons;
using BoneLib;
using Jevil;

namespace BLChaos.Effects;

internal class ZipGun : EffectBase
{
    public ZipGun() : base("Zip Gun", 60) { }
    Vector3 lastShot;
    [RangePreference(0, 100, 1)]
    static float forceMultiplier = 5;

    public override void OnEffectStart() 
    {
        Hooking.OnPostFireGun += Hooking_OnPostFireGun;
    }

    public override void OnEffectEnd()
    {
        Hooking.OnPostFireGun -= Hooking_OnPostFireGun;
    }

    private void Hooking_OnPostFireGun(Gun gun)
    {
        if (!Physics.Raycast(gun.firePointTransform.position, gun.firePointTransform.forward, out RaycastHit hitInfo, 100)) return;

        lastShot = hitInfo.point;
    }

    public override void OnEffectUpdate()
    {
        if (lastShot == default) return;
        Vector3 delta = lastShot - GlobalVariables.Player_PhysRig.rbFeet.transform.position;
        float dist = delta.magnitude;

        GlobalVariables.Player_PhysRig.AddVelocityChange(forceMultiplier * Mathf.Sqrt(dist) * delta);
    }
}
