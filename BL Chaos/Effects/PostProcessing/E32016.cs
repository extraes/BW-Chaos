using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;
using Jevil;
using UnityEngine.Rendering.Universal;

namespace BLChaos.Effects;

internal class E32016 : EffectBase
{
    public E32016() : base("E3 2016", 30, EffectTypes.POST_PROCESS) { }

    VolumeProfile profile;
    public override void OnEffectStart()
    {
        // todo: this doesnt seem like itll actually work. todo: figure out how to programmatically fuck with bloom
        profile = GameObject.FindObjectsOfType<VolumeProfile>().FirstOrDefault(v => v.name == "PostFX Profile");
        Bloom bloom = ScriptableObject.CreateInstance<Bloom>();

        profile.components.Add(bloom);
    }
    public override void OnEffectEnd()
    {
        if (profile.INOC()) return;


    }
}
