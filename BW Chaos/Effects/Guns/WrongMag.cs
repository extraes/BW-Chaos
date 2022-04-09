using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Combat;

namespace BWChaos.Effects
{
    internal class WrongMag : EffectBase
    {
        public WrongMag() : base("Wrong Mag", 60) { }
        static readonly Platform[] platforms = (Platform[])Enum.GetValues(typeof(Platform));
        static readonly Weight[] weights = (Weight[])Enum.GetValues(typeof(Weight));
        AmmoPouch ammoPouch;

        public override void OnEffectStart()
        {
            ammoPouch = GameObject.FindObjectOfType<AmmoPouch>();
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            if (isNetworked) yield break;
            while (Active)
            {
                ammoPouch.UpdateArt(weights.Random());
                ammoPouch.SwitchMagazine(platforms.Random());
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}
