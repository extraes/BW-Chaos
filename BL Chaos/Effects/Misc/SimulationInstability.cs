using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;

using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class SimulationInstability : EffectBase
{
    public SimulationInstability() : base("Simulation Instability", 30) { }

    [RangePreference(2, 20, 2)] static float physTimestepMul = 10;

    public override void OnEffectStart() => Time.fixedDeltaTime *= physTimestepMul;
    public override void OnEffectEnd() => Time.fixedDeltaTime /= physTimestepMul;
}
