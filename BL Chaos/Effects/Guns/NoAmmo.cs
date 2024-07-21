using Jevil;
using SLZ.Combat;
using SLZ.Marrow.Data;
using SLZ.Player;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BLChaos.Effects;

internal class NoAmmo : EffectBase
{
    public NoAmmo() : base("No Ammo") { }

    public override void OnEffectStart()
    {
        AmmoInventory.Instance.ClearAmmo();
    }

#if DEBUG
    internal override Task<TestResult> Test()
    {
        var groupCounts = AmmoInventory.Instance._groupCounts;
        int preAmmoSum = groupCounts.MonoEnumerable().Sum(kvp => kvp.Value);
        OnEffectStart();
        int postAmmoSum = groupCounts.MonoEnumerable().Sum(kvp => kvp.Value);
        Log($"Pre={preAmmoSum}, Post={postAmmoSum}");
        return ResT(postAmmoSum == 0);
    }
#endif
}
