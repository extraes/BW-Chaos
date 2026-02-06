using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Zones;

namespace BLChaos.Effects;

internal class NoMoreChunks : EffectBase, IPatcher
{
    public NoMoreChunks() : base("No More Chunks", 45) { }

    private struct TriggerEvent
    {
        public SceneLoader loader;
        public ChunkBatch cb;
        public bool loaded;
    }

    static bool currentlyActive;
    static List<TriggerEvent> triggers = new();

    public static void Patch()
    {
        Chaos.Instance.HarmonyInstance.Patch(typeof(SceneLoader._LoadChunkBatch_d__11).GetMethod(nameof(SceneLoader._LoadChunkBatch_d__11.MoveNext)), Utilities.ToHarmony(LoadChunkBatchPatch));
        Chaos.Instance.HarmonyInstance.Patch(typeof(SceneLoader._UnloadScenes_d__13).GetMethod(nameof(SceneLoader._UnloadScenes_d__13.MoveNext)), Utilities.ToHarmony(UnloadScenesPatch));
    }

    public override void OnEffectStart()
    {
        currentlyActive = true;
    }

    public override void OnEffectEnd()
    {
        currentlyActive = false;

        foreach (TriggerEvent te in triggers)
        {
            if ((te.loader?.WasCollected ?? true) || (te.cb?.WasCollected ?? true)) continue;

            if (te.loaded)
                te.loader.LoadChunkBatch(te.cb);
            else 
                te.loader.UnloadScenes(te.cb);
        }

        triggers.Clear();
    }

    static bool LoadChunkBatchPatch(SceneLoader._LoadChunkBatch_d__11 __instance)
    {
        if (!currentlyActive || __instance.__1__state != 0)
            return true;

        __instance.__1__state = -1; // "cancels" this unitask

        TriggerEvent tEvent = new()
        {
            loader = __instance.__4__this,
            cb = __instance.chunkBatch,
            loaded = true,
        };
        triggers.Add(tEvent);
        return false;
    }

    static bool UnloadScenesPatch(SceneLoader._UnloadScenes_d__13 __instance)
    {
        if (!currentlyActive)
            return true;

        __instance.__1__state = -1; // "cancels" this unitask

        TriggerEvent tEvent = new()
        {
            loader = __instance.__4__this,
            //todo: why the fuck does that unitask not have a god damn chunkbatch field
            //cb = __instance.,
            loaded = true,
        };
        triggers.Add(tEvent);
        return false;
    }
}
