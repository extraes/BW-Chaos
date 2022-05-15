using ModThatIsNotMod;
using StressLevelZero.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects;

internal class InaccurateGuns : EffectBase
{
    // Don't sync because Discord networking will make this wonky af
    public InaccurateGuns() : base("Inaccurate Guns", 60, EffectTypes.DONT_SYNC) { }
    [RangePreference(0, 60, 2)] static readonly float degreeDeviation = 10;

    public override void OnEffectStart() => Hooking.OnPostFireGun += Hooking_OnPostFireGun;

    public override void OnEffectEnd() => Hooking.OnPostFireGun -= Hooking_OnPostFireGun;

    private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
    {
        StressLevelZero.Combat.Projectile ls = ProjectilePool._instance.lastSpawn;
        Vector3 rot = ls.transform.rotation.eulerAngles;
        rot += Random.insideUnitSphere * degreeDeviation;
        obj.transform.rotation = Quaternion.Euler(rot);
        ls._direction = obj.transform.forward;
    }
}
