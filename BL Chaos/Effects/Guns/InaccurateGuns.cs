using BoneLib;
using Jevil.Patching;
using SLZ.Combat;
using SLZ.Marrow.Pool;
using SLZ.Props.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class InaccurateGuns : EffectBase
{
    // Don't sync because Discord networking will make this wonky af
    public InaccurateGuns() : base("Inaccurate Guns", 60, EffectTypes.DONT_SYNC) { }
    [RangePreference(0, 60, 2)] static readonly float degreeDeviation = 10;
    static Projectile recentProjectile;

    static InaccurateGuns() => Hook.OntoMethod(typeof(Projectile).GetMethod(nameof(Projectile.OnEnable)), (Projectile proj) => recentProjectile = proj);

    public override void OnEffectStart() => Hooking.OnPostFireGun += Hooking_OnPostFireGun;

    public override void OnEffectEnd() => Hooking.OnPostFireGun -= Hooking_OnPostFireGun;

    private void Hooking_OnPostFireGun(Gun obj)
    {
        Projectile ls = recentProjectile;
        Vector3 rot = ls.transform.rotation.eulerAngles;
        rot += Random.insideUnitSphere * degreeDeviation;
        obj.transform.rotation = Quaternion.Euler(rot);
        ls._direction = obj.transform.forward;
    }
}
