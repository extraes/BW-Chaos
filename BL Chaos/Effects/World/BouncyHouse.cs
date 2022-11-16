using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Diagnostics;

namespace BLChaos.Effects;

internal class BouncyHouse : EffectBase
{
    public BouncyHouse() : base("Bouncy House", 90) { }
    Dictionary<Collider, PhysicMaterial> originalMaterials = new Dictionary<Collider, PhysicMaterial>();
    static PhysicMaterial pMat;

    public override void OnEffectStart()
    {
        if (pMat == null)
        {
            pMat = new PhysicMaterial
            {
                hideFlags = HideFlags.DontUnloadUnusedAsset,
                bounciness = 1000,
                bounceCombine = PhysicMaterialCombine.Maximum,
                dynamicFriction = 100,
                staticFriction = 100,
                frictionCombine = PhysicMaterialCombine.Maximum,
            };
        }
    }
    
    public override void OnEffectEnd()
    {
        foreach (var colMat in originalMaterials)
        {
            if (colMat.Key == null) continue;
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
                Log("Waiting a frame after taking 3ms on current frame!");
                yield return null;
                sw.Restart();
            }

            originalMaterials[col] = col.sharedMaterial;
            col.material = pMat;
        }
    }
}
