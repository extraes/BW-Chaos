using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BLChaos;

internal sealed class PreventUnityCrashPreventionCrash /*: IPatcher*/
{
    private PreventUnityCrashPreventionCrash() { }
    public static void Patch()
    {
        // Chaos loads too late for this to do anything. This needs to be either in JeviLib or something that loads before UnityExplorer
        
        //Type? timescalewidget = Type.GetType("UnityExplorer.UI.Widgets.TimeScaleWidget, UnityExplorer.ML.IL2CPP.net6preview.interop, Version=4.9.4.0, Culture=neutral, PublicKeyToken=null");
        //if (timescalewidget is not null)
        //{
        //    Chaos.Log("Patching TimeScaleWidget");
        //    MethodInfo minf = timescalewidget.GetMethod("InitPatch", Jevil.Const.AllBindingFlags)!;
        //    Chaos.Instance.HarmonyInstance.Patch(minf, prefix: Utilities.ToHarmony(FuckOff));
        //}

        Type? crashprevention = Type.GetType("UnityExplorer.Runtime.UnityCrashPrevention, UnityExplorer.ML.IL2CPP.net6preview.interop, Version=4.9.4.0, Culture=neutral, PublicKeyToken=null");
        if (crashprevention is not null)
        {
            Chaos.Log("Patching unitycrashprevention");
            MethodInfo minf = crashprevention.GetMethod("Init", Jevil.Const.AllBindingFlags)!;
            Chaos.Instance.HarmonyInstance.Patch(minf, prefix: Utilities.ToHarmony(FuckOff));
        }

        //Type? reflectionpatches = Type.GetType("UniverseLib.ReflectionPatches, UniverseLib.IL2CPP.Interop.ML, Version=1.5.4.1, Culture=neutral, PublicKeyToken=null");
        //if (reflectionpatches is not null)
        //{
        //    Chaos.Log("Patching reflectionpatches");
        //    MethodInfo minf = reflectionpatches.GetMethod("Init", Jevil.Const.AllBindingFlags)!;
        //    Chaos.Instance.HarmonyInstance.Patch(minf, prefix: Utilities.ToHarmony(FuckOff));
        //}
    }

    static bool FuckOff() => false;
}
