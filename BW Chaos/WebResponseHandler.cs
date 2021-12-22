using BWChaos.Effects;
using MelonLoader;
using StressLevelZero.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BWChaos
{
    internal static class WebResponseHandler
    {
        static readonly FieldInfo effectHandlerSecInfo = typeof(EffectHandler).GetField("secondsEachEffect", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo effectHandlerOverlayTextInfo = typeof(EffectHandler).GetField("candidateText", BindingFlags.Instance | BindingFlags.NonPublic);
        static string _data = "";
        public static void GotData(string data) => _data = data; // proxy because this is ran outside of the main thread

        // use a callback/queue system because unity doesnt like me running shit from off the main thread/from the websocket's async methods
        public static void Callback()
        {
            if (_data == "none" || _data == "") return; // poor/unpopular bitch lol
            if (GlobalVariables.Player_PhysBody == null) return; // dont do shit if the game's loading

            string[] args = Utilities.Argsify(_data, ':');
            _data = "";
            string cmd = args[0];
            string data = args[1];

            switch (cmd)
            {
                case "changerandom":
                    Chaos.Log("changing random effect candidate text to " + data);
                    var txt = (Text)effectHandlerOverlayTextInfo.GetValue(EffectHandler.Instance);
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
                    var pool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name.ToLower().Contains(data.ToLower()));
                    if (pool == null)
                    {
                        Chaos.Warn("admin sent incorrect pool name");
                        return;
                    }
                    var poolee = pool.InstantiatePoolee();
                    Utilities.MoveAndFacePlayer(poolee.gameObject);
                    poolee.gameObject.SetActive(true);
                    break;
                case "runeffect":
                    data = data.ToLower();
                    // use asmEffects because im DEVILISH
                    EffectBase effect = Chaos.asmEffects.FirstOrDefault(e => e.Name.ToLower() == data || e.GetType().Name.ToLower() == data);
                    if (effect == null)
                    {
                        Chaos.Warn("admin tried to run nonexistent effect " + data);
                    }
                    else
                    {
                        if (effect.Types.HasFlag(EffectBase.EffectTypes.USE_STEAM) && !Chaos.isSteamVer)
                        {
                            Chaos.Warn("admin tried to run incompatible effect (youre on oculus) " + data);
                        }
                        var newE = (EffectBase)Activator.CreateInstance(effect.GetType());
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

                default:
                    Chaos.Warn("admin sent nonexistent command lol: " + cmd);
                    break;
            }
        }
    }
}
