using MelonLoader;
using UnityEngine;
using static BWChaos.Utilities;

namespace BWChaos.Effects;

internal class ShadyURL : EffectBase
{
    public ShadyURL() : base("Shady URL", 15) { }
    [EffectPreference] static readonly bool showInSteamOverlayIfAvailable = true;

    private readonly string[] shadyURLs = new string[] {
        "http://www.5z8.info/begin-bank-account-xfer_o8u1tn_cockfights",            // https://www.youtube.com/embed/5IVsAcWqTc8?rel=0 GROUSE
        "http://www.5z8.info/hitler_g4v2bu_peepshow",                               // http://endless.horse/ hooooooooooooooooooooooooooooooooooooooorse
        "http://www.5z8.info/-php-deactivate_phishing_filter-48-_k0k8ee_smut",      // https://froggi.es/website/site/ I like Shirts (or whatever) god tier site tho
        "http://www.5z8.info/56-DEPLOY-TROJAN-287.mw9----_g9v0ut_facebook-hack",    // https://longdogechallenge.com/ infinidoge
        "http://www.5z8.info/barely-legal_r7p9ws_add-worm",                         // http://eelslap.com/ eelslap!
        "http://www.5z8.info/hack-outlook_b9r3vo_boobs",                            // http://corndog.io/ a bunch of corndogs floating in space
        "http://www.5z8.info/facebook-hack_k8o3fv_start-trojan"                     // https://bullsquid.com/ spinning bullsquid from hl1
    };

    public override void HandleNetworkMessage(string data)
    {
        MelonCoroutines.Start(CoShow(data));
    }

    [AutoCoroutine]
    public System.Collections.IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        string pickedURL = shadyURLs.Random();
        SendNetworkData(pickedURL);
        yield return CoShow(pickedURL);
    }

    public System.Collections.IEnumerator CoShow(string url)
    {
        SpawnAd("Hey wanna see one of my favorite websites");
        yield return new WaitForSeconds(4f);
        SpawnAd(url);

        if (Chaos.isSteamVer && showInSteamOverlayIfAvailable)
        {
            yield return new WaitForSeconds(5f);
            SpawnAd("Why don't I show you :^)");
            yield return new WaitForSeconds(5f);

            Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url, Steamworks.EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
        }
    }
}
