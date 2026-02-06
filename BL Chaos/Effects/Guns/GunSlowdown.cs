#if !NOBONELIB
namespace BLChaos.Effects;

class GunSlowdown : EffectBase
{
    public GunSlowdown() : base("Progressively slower guns", 60) { }
    [RangePreference(0, 1, 0.01f)] static float multiplier = 0.99f;

    public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
    public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

    public void OnGunFired(Gun gun)
    {
        GameObject? leftHandObj = Player.GetObjectInHand(Player.LeftHand);
        GameObject? rightHandObj = Player.GetObjectInHand(Player.RightHand);
        if (leftHandObj == null && rightHandObj == null) return;

        if (gun.transform.IsChildOf(leftHandObj.transform) || gun.transform.IsChildOf(rightHandObj.transform))
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
#endif
