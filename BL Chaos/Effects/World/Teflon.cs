using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Diagnostics;

namespace BLChaos.Effects;

internal class Teflon : EffectBase
{
    public Teflon() : base("Teflon", 90) { }
    Dictionary<Collider, PhysicMaterial> originalMaterials = new Dictionary<Collider, PhysicMaterial>();
    static PhysicMaterial pMat;

    public override void OnEffectStart()
    {
        if (pMat == null)
        {
            pMat = new PhysicMaterial
            {
                hideFlags = HideFlags.DontUnloadUnusedAsset,
                bounciness = 0,
                frictionCombine = PhysicMaterialCombine.Multiply,
                dynamicFriction = 0,
                staticFriction = 0,
            };
        }

        Utilities.SpawnAd("New <b>Du Pont</b> Teflon!\n<i>Now with <s>no</s> less poison!</i>");
    }
    
    public override void OnEffectEnd()
    {
        foreach (var colMat in originalMaterials)
        {
            colMat.Key.material = colMat.Value;
        }
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        Stopwatch sw = Stopwatch.StartNew();

        var cols = GameObject.FindObjectsOfType<Collider>();
        Log($"Changing {cols.Length} collider materials");
        foreach (Collider col in cols)
        {
            if (sw.ElapsedMilliseconds > 3) // only take up 3ms of frame time.
            {
                yield return null;
                sw.Restart();
            }

            originalMaterials[col] = col.sharedMaterial;
            col.material = pMat;
        }
    }
}
