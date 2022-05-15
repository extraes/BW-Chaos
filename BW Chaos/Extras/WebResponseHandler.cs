using BWChaos.Effects;
using StressLevelZero.Pool;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BWChaos.Extras;

internal static class WebResponseHandler
{
    static readonly FieldInfo effectHandlerSecInfo = typeof(EffectHandler).GetField("secondsEachEffect", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo effectHandlerOverlayTextInfo = typeof(EffectHandler).GetField("candidateText", BindingFlags.Instance | BindingFlags.NonPublic);
    static string data = "";
    public static void GotData(string _data) => data = _data; // proxy because this is ran outside of the main thread

    // use a callback/queue system because unity doesnt like me running shit from off the main thread/from the websocket's async methods
    public static void Callback()
    {
        if (WebResponseHandler.data == "none" || WebResponseHandler.data == "") return; // poor/unpopular bitch lol
        if (GlobalVariables.Player_PhysBody == null) return; // dont do shit if the game's loading
        Chaos.Log("Wow! You must be popular or something! The creator's taken notice of you playing their mod!");

        string[] args = Utilities.Argsify(WebResponseHandler.data, ':');
        string cmd = args[0];
        string data = args[1];
        WebResponseHandler.data = "";

        switch (cmd)
        {
            case "changerandom":
                Chaos.Log("changing random effect candidate text to " + data);
                Text txt = (Text)effectHandlerOverlayTextInfo.GetValue(EffectHandler.Instance);
                txt.text = txt.text.Replace("Random Effect", data);
                break;
            case "spawnsign":
                Chaos.Log("spawning ad from admin with text " + data);
                Utilities.SpawnAd(data);
                break;
            case "melonlog":
                Chaos.Log(data);
                break;
            case "spawnfrompool":
                Chaos.Log("Trying to spawn an object from a pool called " + data);
                Pool pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name.ToLower() == data.ToLower());
                if (pool == null)
                {
                    Chaos.Warn("admin sent incorrect pool name " + data);
                    return;
                }
                Poolee poolee = pool.InstantiatePoolee(Vector3.one, Quaternion.identity);
                poolee.gameObject.SetActive(true);
                Utilities.MoveAndFacePlayer(poolee.gameObject);
#if DEBUG
                Chaos.Log($"Spawned {poolee.name} at {poolee.transform.position}, active = {poolee.isActiveAndEnabled}");
#endif
                break;
            case "runeffect":
                data = data.ToLower();
                // use asmEffects because im DEVILISH and DONT CARE about your MELONPREFERENCES
                EffectBase effect = Chaos.asmEffects.FirstOrDefault(e => e.Name.ToLower() == data || e.GetType().Name.ToLower() == data);
                if (effect == null)
                {
                    Chaos.Warn("admin tried to run nonexistent effect " + data);
                }
                else
                {
                    if (effect.Types.HasFlag(EffectBase.EffectTypes.USE_STEAM) && !Chaos.isSteamVer)
                    {
                        Chaos.Warn("admin tried to run incompatible effect (youre on oculus) - " + data);
                    }
                    EffectBase newE = (EffectBase)Activator.CreateInstance(effect.GetType());
                    newE.Run();
                }
                break;
            case "timerspeed":
                if (int.TryParse(data, out int value))
                {
                    effectHandlerSecInfo.SetValue(EffectHandler.Instance, value);
                }
                else Chaos.Warn("admin sent non-int value with timerspeed " + data);
                break;
            case "fold":
                Chaos.Warn("IDK Why, but you've been told to stop, by force! Closing the game now!");
                Application.Quit();
                break;
            default:
                Chaos.Warn("admin sent nonexistent command lol: " + cmd);
                break;
        }
    }
}
