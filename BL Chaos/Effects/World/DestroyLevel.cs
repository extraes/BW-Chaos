using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace BLChaos.Effects;

internal class DestroyLevel : EffectBase
{
    public DestroyLevel() : base("Slowly Destroy Level Pieces", 60) { }
    [RangePreference(0, 2, 0.125f)]
    static float waitTime = 0.25f;
    List<GameObject> disabledObjects = new();

    public override void HandleNetworkMessage(string data)
    {
        GameObject go = GameObject.Find(data);
        if (go == null) return;

        disabledObjects.Add(go);
        go.SetActive(false);
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        if (isNetworked) yield break;
        List<MeshFilter> filters = GameObject.FindObjectsOfType<MeshFilter>()
                                          .Where(f => f.mesh.subMeshCount > 5) // only combined meshes have a high submesh count
                                          .ToList();
#if DEBUG
        Log("Found " + filters.Count + " static meshes");
#endif

        while (Active)
        {
            GameObject chosenGo = filters.Random().gameObject;
            disabledObjects.Add(chosenGo);
            chosenGo.SetActive(false);

#if DEBUG
            Log("Disabling " + chosenGo.name);
#endif
            SendNetworkData(chosenGo.transform.GetFullPath());
            yield return null;
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    public override void OnEffectEnd()
    {
        foreach (GameObject go in disabledObjects)
        {
            if (go.WasCollected || go == null) continue;
            go.SetActive(true);
        }
    }

    //static bool GetRend(Collider input)
    //{
    //    MeshFilter phil = input.GetComponentInParent<MeshFilter>();
    //    if (phil != null )
    //}
}
