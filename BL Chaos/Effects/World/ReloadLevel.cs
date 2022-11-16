using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using SLZ.Utilities;
using Utilities;
using SLZ.Marrow.SceneStreaming;

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

        if (currTime > 5 * 60) SceneStreamer.Reload();
        lastTime1FrameAgo = currTime;
    }
}
