using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using SLZ.Marrow.SceneStreaming;
using Jevil;
using System.Collections.Generic;
using static MelonLoader.MelonLogger;

namespace BLChaos.Effects;

internal class NoMoreChunks : EffectBase
{
    public NoMoreChunks() : base("No More Chunks", 45) { }

    private struct TriggerEvent
    {
        public ChunkTrigger trigger;
        public Collider col;
        public bool entered;
    }

    static bool CurrentlyActive;
    static List<TriggerEvent> triggers = new();

    static NoMoreChunks()
    {
        Chaos.Instance.HarmonyInstance.Patch(typeof(ChunkTrigger).GetMethod(nameof(ChunkTrigger.OnTriggerEnter)), Utilities.ToHarmony(OnTriggerEnterPatch));
    }

    public override void OnEffectStart()
    {
        CurrentlyActive = true;
    }

    public override void OnEffectEnd()
    {
        CurrentlyActive = false;

        foreach (TriggerEvent te in triggers)
        {
            if (te.trigger.INOC() || te.col.INOC()) continue;

            if (te.entered)
                te.trigger.OnTriggerEnter(te.col);
            else 
                te.trigger.OnTriggerExit(te.col);
        }

        triggers.Clear();
    }

    static bool OnTriggerEnterPatch(ChunkTrigger __instance, Collider other)
    {
        if (!CurrentlyActive)
            return true;

        TriggerEvent tEvent = new()
        {
            trigger = __instance,
            col = other,
            entered = true,
        };
        triggers.Add(tEvent);
        return false;
    }

    static bool OnTriggerExitPatch(ChunkTrigger __instance, Collider other)
    {
        if (!CurrentlyActive)
            return true;

        TriggerEvent tEvent = new()
        {
            trigger = __instance,
            col = other,
            entered = false,
        };
        triggers.Add(tEvent);
        return false;

    }
}
