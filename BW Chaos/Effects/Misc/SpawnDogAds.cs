﻿using UnityEngine;
using System.Collections;

namespace BWChaos.Effects
{
    internal class SpawnDogAd : EffectBase
    {
        public SpawnDogAd() : base("Spawn Dog Ads", 75) { }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            while (Active)
            {
                // cant sync this unless i try to patch MTINM or implement chap's logic in my own code. Oh well.
                ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
                yield return new WaitForSecondsRealtime(2.5f);
            }
        }
    }
}
