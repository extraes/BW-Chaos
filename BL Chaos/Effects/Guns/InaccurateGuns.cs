using BoneLib;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Jevil.Patching;
using Jevil.Spawning;
using SLZ.Combat;
using SLZ.Marrow.Pool;
using SLZ.Props.Weapons;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Awake))]
internal class InaccurateGuns : EffectBase
{
    // Don't sync because Discord networking will make this wonky af
    public InaccurateGuns() : base("Inaccurate Guns", 60, EffectTypes.DONT_SYNC) { }
    [RangePreference(0, 60, 2)] static readonly float degreeDeviation = 10;
    static Projectile recentProjectile;

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

    [HarmonyPostfix]
    private static void Projectile_Awake(Projectile __instance)
    {
        // i will be fucking SHOCKED if that WORKS;
        __instance.onCreate.AddListener(new Action(() => { recentProjectile = __instance; }));
    }

#if DEBUG
    internal override async Task<TestResult> Test()
    {
        AssetPoolee poolee = await Barcodes.SpawnAsync(JevilBarcode.MP5, Vector3.zero, Quaternion.identity);
        Gun mp5 = poolee.GetComponentInChildren<Gun>();
        mp5.ForceFireable();
        mp5.Fire();
        if (recentProjectile == null) Log("RecentProj was null immediately after Fire.");
        await UniTask.Yield(PlayerLoopTiming.Update);
        if (recentProjectile == null) throw new NullReferenceException("Fix ForceFireable! RecentProjectile was null.");

        Vector3 preforward = recentProjectile._direction;
        OnEffectStart();
        mp5.Fire();
        await UniTask.Yield(PlayerLoopTiming.Update);
        Vector3 postforward = recentProjectile._direction;
        OnEffectEnd();
        Log($"preforward = {preforward}, postforward = {postforward}, deg diff = {Vector3.Angle(preforward, postforward)}");
        return Res(preforward != postforward);
    }
#endif
}
