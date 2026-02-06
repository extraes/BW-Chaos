#if !NOBONELIB
namespace BLChaos.Effects;

class GunSpeedup : EffectBase
{
    // If a gun gets too fast, the pooled bullets will be reused before they can hit anything... Oops!
    public GunSpeedup() : base("Progressively faster guns", 60) { }
    [RangePreference(1, 2, 0.01f)] static float multiplier = 1.01f;
    public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
    public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

    public void OnGunFired(Gun gun)
    {

        //if (gun == Player.GetGunInHand(Player.RightHand) || gun == Player.GetGunInHand(Player.LeftHand))
        //{
#if DEBUG
        Chaos.Log($"GUN FIRED! RPM={gun.roundsPerMinute}; PATH: {gun.transform.GetFullPath()}");
#endif
        gun.SetRpm(gun.roundsPerMinute * multiplier);
        //}
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
#endif