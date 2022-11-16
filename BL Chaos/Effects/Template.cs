using System;
using UnityEngine;
using MelonLoader;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class Template : EffectBase
{
    public Template() : base("Template Effect") { }

    public override void HandleNetworkMessage(string data) => Log("I got some data! " + data);
    public override void OnEffectStart() => Log("Placeholder start");
    public override void OnEffectUpdate() => Log("Placeholder update");
    public override void OnEffectEnd() => Log("Placeholder end");
}
