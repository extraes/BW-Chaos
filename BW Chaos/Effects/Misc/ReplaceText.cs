using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Reflection;

namespace BWChaos.Effects
{
    internal class ReplaceText : EffectBase
    {
        public ReplaceText() : base("Replace a quarter of all text") { }
        private static string[] spawnAdStrings = typeof(SpawnAds).GetField("ads", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as string[];


        public override void OnEffectStart()
        {
            foreach (var tmp in GameObject.FindObjectsOfType<TMPro.TMP_Text>())
            {
                if (UnityEngine.Random.value < 0.25f)
                {
                    tmp.text = spawnAdStrings.Random();
                }
            }
        }
    }
}
