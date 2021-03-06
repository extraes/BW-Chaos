using Boneworks;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects;

internal class ScrollingTextures : EffectBase
{
    public ScrollingTextures() : base("IN YOUR WALLS.", 5) { }
    [RangePreference(0, 1, 0.0125f)] static readonly float scrollSpeedX = 0.025f;
    [RangePreference(0, 1, 0.0125f)] static readonly float scrollSpeedY = 0.05f;
    [RangePreference(0, 1, 0.02f)] static readonly float swapChance = 0.2f;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        bool stagger = true;

        foreach (MeshRenderer mesh in Utilities.FindAll<MeshRenderer>())
        {
            if (mesh.gameObject.GetComponent<Treadmill>() != null) continue;
            if (mesh == null || !mesh.gameObject.active) continue;

            if (Random.value < swapChance)
            {
                if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                if (mesh.GetComponent<TMPro.TMP_Text>() != null) continue;


                Treadmill tread = mesh.gameObject.AddComponent<Treadmill>();
                tread.directionMill = new Vector2(scrollSpeedX, scrollSpeedY);
                tread.materialMill = mesh?.material;
                SendNetworkData(mesh.transform.GetFullPath());
                if (stagger = !stagger) yield return null;
            }
        }
    }

    public override void HandleNetworkMessage(string data)
    {
        string[] args = data.Split(';');


        GameObject go = GameObject.Find(args[1]);
        if (go == null)
        {
            Chaos.Warn("GameObject was not found in client: " + args[1]);
            return;
        }

        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        if (mesh == null)
        {
            Chaos.Warn("The recieved GameObject didn't have a MeshRenderer");
            return;
        }
        Treadmill tread = mesh.gameObject.AddComponent<Treadmill>();
        tread.materialMill = mesh.material;
        tread.directionMill = new Vector2(0.025f, 0.05f);
    }
}
