#if DEBUG
using BoneLib.RandomShit;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BLChaos;

internal static class TestingHelper
{
    // this does nothing. it should probably stay that way.
    public static void Init()
    {
        if (Prefs.lastEffectTested.Value == -1)
            return;

        Start();
    }

    public static void Start()
    {
        if (Prefs.lastEffectTested.Value == -1)
            Prefs.lastEffectTested.Value = 0;
        Chaos.Log("Starting effect testing circuit!");
        MelonCoroutines.Start(CoTest());
    }

    static IEnumerator CoTest()
    {
        foreach (var kvp in EffectHandler.allEffects.Skip(Prefs.lastEffectTested.Value).ToArray())
        {
            Chaos.Log($"Starting effect: {kvp.Key} - Dur={kvp.Value.Duration} sec, Flags={kvp.Value.Types}");

            kvp.Value.Run();

            Prefs.lastEffectTested.Value++;

            float waitTime = Prefs.waitEffectDurForTest ? kvp.Value.Duration + 5 : 30;
            yield return new WaitForSecondsRealtime(waitTime);
        }

        Chaos.Log("All effects ran!");
        PopupBoxManager.CreateNewPopupBox("All effects ran! You can now rest peacefully.");
        Prefs.lastEffectTested.Value = -1;
    }
}
#endif
