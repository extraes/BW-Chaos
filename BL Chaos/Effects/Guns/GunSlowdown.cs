using BoneLib;
using SLZ.Props.Weapons;
using System.Threading.Tasks;
using UnityEngine;

namespace BLChaos.Effects;

class GunSlowdown : EffectBase
{
    public GunSlowdown() : base("Progressively slower guns", 60) { }
    [RangePreference(0, 1, 0.01f)] static readonly float multiplier = 0.99f;

    public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
    public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

    public void OnGunFired(Gun gun)
    {
        if (gun == Player.GetGunInHand(Player.rightHand) || gun == Player.GetGunInHand(Player.leftHand))
        {
            gun.SetRpm(gun.roundsPerMinute * multiplier);
        }
    }

#if DEBUG
    internal override Task<TestResult> Test()
    {
        OnEffectStart();
        Gun gun = new GameObject("guntmp").AddComponent<Gun>();
        float preRpm = gun.roundsPerMinute;
        gun.Fire();
        float postRpm = gun.roundsPerMinute;
        OnEffectEnd();
        gun.gameObject.Destroy();
        return ResT(preRpm != postRpm);
    }
#endif
}
