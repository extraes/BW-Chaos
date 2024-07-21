using BoneLib;
using Cysharp.Threading.Tasks;
using Jevil;
using SLZ.Props.Weapons;
using System.Threading.Tasks;
using UnityEngine;

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

#if DEBUG
    internal override async Task<TestResult> Test()
    {
        GlobalVariables.Player_PhysRig.AddVelocityChange(Vector3.up * 100);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        return Res(GlobalVariables.Player_PhysRig.torso.rbPelvis.velocity.magnitude > 50);
    }
#endif
}
