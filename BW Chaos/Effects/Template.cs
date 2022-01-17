using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class Template : EffectBase
    {
        public Template() : base("Template Effect") { }

        public override void HandleNetworkMessage(string data) => Chaos.Log("I got some data! " + data);
        public override void OnEffectStart() => Chaos.Log("Placeholder start");
        public override void OnEffectUpdate() => Chaos.Log("Placeholder update");
        public override void OnEffectEnd() => Chaos.Log("Placeholder end");
    }
}
