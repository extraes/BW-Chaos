using System;
using System.Collections;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class FuckYourMag : EffectBase
    {
        public FuckYourMag() : base("Fuck Your Magazine", 90, EffectTypes.HIDDEN) { }
        [RangePreference(1, 10, 1)] static float minWaitTime = 5;
        [RangePreference(10,20,1)] static float maxWaitTime = 10f;

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            while (Active)
            {
                Gun gun = Player.GetGunInHand(Utilities.GetRandomPlayerHand());

                gun?.magazineSocket?.MagazineRelease();

                yield return new WaitForSecondsRealtime(UnityEngine.Random.RandomRange(minWaitTime, maxWaitTime));
            }
        }
    }
}
