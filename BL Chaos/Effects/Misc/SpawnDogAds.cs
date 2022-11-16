using BoneLib.RandomShit;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http;
using UnityEngine;

namespace BLChaos.Effects;

internal class SpawnDogAd : EffectBase
{
    public SpawnDogAd() : base("Spawn Dog Ads", 75) { }
    [RangePreference(0.25f, 10, 0.25f)] static readonly float waitTime = 2.5f;
    const string API = "http://shibe.online/api/shibes";
    static readonly HttpClient httpClient = new();

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        while (Active)
        {
            var imageUrlTask = httpClient.GetStringAsync(API);
            while (!imageUrlTask.IsCompleted) yield return null;
            string imageUrlJson = imageUrlTask.Result;
            string imageUrl = JsonConvert.DeserializeObject<string[]>(imageUrlJson).Random();
            
            var imageBytesTask = httpClient.GetByteArrayAsync(imageUrl);
            while (!imageBytesTask.IsCompleted) yield return null;
            byte[] image = imageBytesTask.Result;
            // cant sync this unless i try to patch MTINM or implement chap's logic in my own code. Oh well.
            PopupBoxManager.CreateNewImagePopup(image);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}
