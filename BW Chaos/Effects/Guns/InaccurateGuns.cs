using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.Pool;

namespace BWChaos.Effects
{
    internal class InaccurateGuns : EffectBase
    {
        // Don't sync because Discord networking will make this wonky af
        public InaccurateGuns() : base("Inaccurate Guns", 60, EffectTypes.DONT_SYNC) { }
        [RangePreference(0, 60, 2)] static float degreeDeviation = 10;

        public override void OnEffectStart() => Hooking.OnPostFireGun += Hooking_OnPostFireGun;

        public override void OnEffectEnd() => Hooking.OnPostFireGun -= Hooking_OnPostFireGun;

        private void Hooking_OnPostFireGun(StressLevelZero.Props.Weapons.Gun obj)
        {
            var ls = ProjectilePool._instance.lastSpawn;
            var rot = ls.transform.rotation.eulerAngles;
            rot += Random.insideUnitSphere * degreeDeviation;
            obj.transform.rotation = Quaternion.Euler(rot);
            ls._direction = obj.transform.forward;
        }
    }
}
