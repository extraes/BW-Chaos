using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using StressLevelZero.Utilities;

namespace BLChaos.Effects;

internal class ReloadLevel : EffectBase
{
    public ReloadLevel() : base("Reload level in 5 min", 301, EffectTypes.DONT_SYNC | EffectTypes.META) { }
    [EffectPreference]
    static bool warningSigns = true;
    float currTime;
    float lastTime1FrameAgo;

    public override void OnEffectUpdate()
    {
        currTime += Time.deltaTime;

        int passed30s = (int)currTime / 30;
        int passed30sLast = (int)lastTime1FrameAgo / 30;
        if (passed30s != passed30sLast && warningSigns)
        {
            Utilities.SpawnAd(300 - (30 * passed30s) + " seconds left...");
        }

        if (currTime > 5 * 60) BoneworksSceneManager.ReloadScene();
        lastTime1FrameAgo = currTime;
    }
}
