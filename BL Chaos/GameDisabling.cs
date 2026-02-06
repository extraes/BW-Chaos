using BLChaos.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLChaos;

/// <summary>
/// As an effect, add yourself to the list corresponding to the thing you want to prevent.
/// </summary>
internal sealed class GameDisabling : IPatcher
{
    private GameDisabling() { }
    public static void Patch()
    {
        Chaos.Log("Patching TimeManager.DECREASE_TIMESCALE");
        Chaos.Instance.HarmonyInstance.Patch(Utilities.AsInfo(TimeManager.DECREASE_TIMESCALE), Utilities.ToHarmony(TimeManagerDecreaseTimescale));
    }

    public static List<EffectBase> ActivateSlowmo = new();
    static bool TimeManagerDecreaseTimescale() => ActivateSlowmo.Count == 0; // lets it run if there's nothing blocking it
}
