using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Reflection;
using TMPro;

namespace BWChaos.Effects
{
    internal class ReplaceText : EffectBase
    {
        public ReplaceText() : base("Replace text") { }
        private static string[] spawnAdStrings = typeof(SpawnAds).GetField("ads", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as string[];
        [RangePreference(0, 1f, 0.05f)] static float replaceChance = 0.25f;


        public override void OnEffectStart()
        {
            if (isNetworked) return;
            foreach (var tmp in GameObject.FindObjectsOfType<TMP_Text>())
            {
                if (UnityEngine.Random.value < replaceChance)
                {
                    string txt = spawnAdStrings.Random();
                    tmp.text = txt;
                    SendNetworkData(tmp.transform.GetFullPath() + ";" + txt);
                }
            }
        }

        public override void HandleNetworkMessage(string data)
        {
            string[] datas = data.Split(';');
            var tmp = GameObject.Find(datas[0])?.GetComponent<TMP_Text>();
            if (tmp == null)
            {
#if DEBUG
                Chaos.Warn($"Path DNE/TMP DNE on path \"{datas[0]}\"");
#endif
                return;
            }
            tmp.text = datas[1];
        }
    }
}
