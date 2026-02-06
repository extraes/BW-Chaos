#if !NOBONELIB
using BoneLib.RandomShit;

namespace BLChaos.Effects;

internal class SpawnDogAd : EffectBase
{
    public SpawnDogAd() : base("Spawn Dog Ads", 75) { }
    [RangePreference(0.25f, 10, 0.25f)] static float waitTime = 2.5f;
    const string API = "http://shibe.online/api/shibes";
    static readonly HttpClient httpClient = new();

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        while (Active)
        {
            PopupBoxManager.CreateNewShibePopup();
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}
#endif