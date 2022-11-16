

using BoneLib;

namespace BLChaos.Effects;

class GunSpeedup : EffectBase
{
    // If a gun gets too fast, the pooled bullets will be reused before they can hit anything... Oops!
    public GunSpeedup() : base("Progressively faster guns", 60) { }
    [RangePreference(1, 2, 0.01f)] static readonly float multiplier = 1.01f;
    public override void OnEffectStart() => Hooking.OnPostFireGun += OnGunFired;
    public override void OnEffectEnd() => Hooking.OnPostFireGun -= OnGunFired;

    public void OnGunFired(SLZ.Props.Weapons.Gun gun)
    {
        if (gun == Player.GetGunInHand(Player.rightHand) || gun == Player.GetGunInHand(Player.leftHand))
        {
            gun.SetRpm(gun.roundsPerMinute * multiplier);
        }
    }
}
