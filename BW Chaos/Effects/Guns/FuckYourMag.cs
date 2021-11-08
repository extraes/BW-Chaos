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

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            while (Active)
            {
                Gun gun = Player.GetGunInHand(Utilities.GetRandomPlayerHand());

                gun?.magazineSocket?.MagazineRelease();

                yield return new WaitForSecondsRealtime(UnityEngine.Random.RandomRange(5f,10f));
            }
        }
    }
}
