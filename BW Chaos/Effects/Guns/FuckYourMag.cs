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
        public FuckYourMag() : base("Fuck Your Magazine", 90) { }

        public override void OnEffectStart() => MelonCoroutines.Start(CoRun());

        private IEnumerator CoRun()
        {
            while (Active)
            {
                Gun gun = UnityEngine.Random.Range(0, 2) == 1
                ? Player.GetGunInHand(Player.leftHand)
                : Player.GetGunInHand(Player.rightHand);

                gun?.magazineSocket?.MagazineRelease();

                yield return new WaitForSecondsRealtime(UnityEngine.Random.RandomRange(5f,10f));
            }
        }
    }
}
