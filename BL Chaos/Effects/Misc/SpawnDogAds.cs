using System.Collections;
using UnityEngine;

namespace BLChaos.Effects;

internal class SpawnDogAd : EffectBase
{
    public SpawnDogAd() : base("Spawn Dog Ads", 75) { }
    [RangePreference(0.25f, 10, 0.25f)] static readonly float waitTime = 2.5f;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        while (Active)
        {
            // cant sync this unless i try to patch MTINM or implement chap's logic in my own code. Oh well.
            ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}
