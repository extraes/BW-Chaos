using BWChaos.Effects;
using StressLevelZero.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BWChaos
{
    internal static class WebResponseHandler
    {
        static readonly FieldInfo effectHandlerSecInfo = typeof(EffectHandler).GetField("secondsEachEffect", BindingFlags.Instance | BindingFlags.NonPublic);
        public static void Handle(string _data)
        {
            if (_data == "none") return; // poor/unpopular bitch lol
            if (GlobalVariables.Player_PhysBody == null) return; // dont do shit if the game's loading

            string[] args = Utilities.Argsify(_data, ':');
            string cmd = args[0];
            string data = args[1];

            switch (cmd)
            {
                case "spawnsign":
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
                    }
                    break;
                case "runeffect":
                    data = data.ToLower();
                    EffectBase effect = EffectHandler.AllEffects.Values.FirstOrDefault(e => e.Name.ToLower() == data || e.GetType().Name.ToLower() == data);
                    if (effect == null)
                    {
                        Chaos.Warn("admin tried to run nonexistent effect " + data);
                    }
                    else
                    {
                        var newE = (EffectBase)Activator.CreateInstance(effect.GetType());
                        newE.Run();
                    }
                    break;
                case "timerspeed":
                    if (int.TryParse(data, out int value)) {
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
