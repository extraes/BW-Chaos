using System.Reflection;
using TMPro;
using UnityEngine;

namespace BWChaos.Effects;

internal class ReplaceText : EffectBase
{
    public ReplaceText() : base("Replace text") { }
    private static readonly string[] spawnAdStrings = typeof(SpawnAds).GetField("ads", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as string[];
    [RangePreference(0, 1f, 0.05f)] static readonly float replaceChance = 0.25f;


    public override void OnEffectStart()
    {
        if (isNetworked) return;
        foreach (TMP_Text tmp in GameObject.FindObjectsOfType<TMP_Text>())
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
        TMP_Text tmp = GameObject.Find(datas[0])?.GetComponent<TMP_Text>();
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
