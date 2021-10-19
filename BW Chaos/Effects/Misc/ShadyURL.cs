using System;
using UnityEngine;
using MelonLoader;
using static BWChaos.Utilities;

namespace BWChaos.Effects
{
    internal class ShadyURL : EffectBase
    {
        public ShadyURL() : base("Shady URL", 15) { }

        public override void OnEffectStart()
        {
            MelonCoroutines.Start(CoRun());
        }

        private string[] shadyURLs = new string[] {
            "http://www.5z8.info/begin-bank-account-xfer_o8u1tn_cockfights",            // https://www.youtube.com/embed/5IVsAcWqTc8?rel=0 GROUSE
            "http://www.5z8.info/hitler_g4v2bu_peepshow",                               // http://endless.horse/ hooooooooooooooooooooooooooooooooooooooorse
            "http://www.5z8.info/-php-deactivate_phishing_filter-48-_k0k8ee_smut",      // https://froggi.es/website/site/ I like Shirts (or whatever) god tier site tho
            "http://www.5z8.info/56-DEPLOY-TROJAN-287.mw9----_g9v0ut_facebook-hack",    // https://longdogechallenge.com/ infinidoge
            "http://www.5z8.info/barely-legal_r7p9ws_add-worm",                         // http://eelslap.com/ eelslap!
            "http://www.5z8.info/hack-outlook_b9r3vo_boobs",                            // http://corndog.io/ a bunch of corndogs floating in space
        };

        private System.Collections.IEnumerator CoRun ()
        {
            yield return null;
            var pickedURL = shadyURLs[UnityEngine.Random.RandomRange(0, shadyURLs.Length)];
            SpawnAd("Hey wanna see one of my favorite websites");
            yield return new WaitForSeconds(5f);
            SpawnAd(pickedURL);

            if (BWChaos.isSteamVer)
            {
                yield return new WaitForSeconds(5f);
                SpawnAd("Why don't I show you :^)");
                yield return new WaitForSeconds(5f);
                
                Steamworks.SteamFriends.ActivateGameOverlayToWebPage(pickedURL, Steamworks.EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
            }
        }
    }
}
