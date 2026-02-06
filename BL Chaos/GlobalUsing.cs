#if !NOBONELIB
global using BoneLib;
#endif

global using Il2CppCysharp.Threading.Tasks;
global using Jevil;
global using Jevil.Spawning;
global using Il2CppSLZ.Marrow.Data;
global using Il2CppSLZ.Marrow.Pool;
global using System.Text;
global using System.Collections;
global using UnityEngine;
global using HarmonyLib;
global using MelonLoader;
global using Il2CppSLZ.Marrow;
global using Il2Cpp;
global using Il2CppTMPro;
global using DebugDraw = Jevil.IMGUI.DebugDraw;
global using Random = UnityEngine.Random;
using Il2CppSLZ.Bonelab;
using BLChaos;

[HarmonyPatch(typeof(BoardGenerator._BoardSpawnerAsync_d__29), nameof(BoardGenerator._BoardSpawnerAsync_d__29.MoveNext))]
public static class thepatch // eek
{
    public static void Prefix(BoardGenerator._BoardSpawnerAsync_d__29 __instance)
    {
        Chaos.Log("PREFIXED BoardGenerator._BoardSpawnerAsync_d__29.MoveNext");
        Chaos.Log(" - State=" + __instance.__1__state);
    }

    public static void Postfix(BoardGenerator._BoardSpawnerAsync_d__29 __instance)
    {
        Chaos.Log("POSTFIXED BoardGenerator._BoardSpawnerAsync_d__29.MoveNext");
        Chaos.Log(" - State=" + __instance.__1__state);
    }
}