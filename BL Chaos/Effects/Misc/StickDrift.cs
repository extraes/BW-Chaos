using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using BoneLib;
using Jevil;

namespace BLChaos.Effects;

internal class StickDrift : EffectBase
{
    public StickDrift() : base("Stick Drift", 15) { }

    public override void OnEffectStart()
    {
        float secFromNow = Random.Range(0f, 5f);
        float dur = Random.Range(Duration / 2, Duration);
        float amp = Random.Range(0.1f, 0.9f);
        Utilities.GetRandomPlayerHand().Controller.haptor.SENDHAPTIC(secFromNow, dur, 60, amp);
    }
}
