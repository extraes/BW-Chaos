using MelonLoader;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class CaughtIn4K : EffectBase
    {
        public CaughtIn4K() : base("Caught in 4K UHD Dolby HDR10 H.265 HEVC With Dolby Atmos Surround Sound for Headphones", 120) { }

        public override void OnEffectStart() => MelonCoroutines.Start(CoRun());

        bool wasCaught = false;
        CSteamID userID;
        private IEnumerator CoRun()
        {
            yield return null;
            spawnAd("let's see here...", false);
            yield return new WaitForSecondsRealtime(5f);

            if (Directory.GetCurrentDirectory().Contains("r2modman"))
            {
                spawnAd("I see you're using r2modman...");
                yield return new WaitForSecondsRealtime(8f);
                spawnAd("not that you asked, but once i was against r2modman");
                spawnAd("it took some control out of the hands of mod makers");
                spawnAd("but now i dont really care because theres less 'how do i extract a 7z' questions", false);
                yield return new WaitForSecondsRealtime(8f);
            }

            #region Read start menu shortcuts

            // Get entries from User's start menu
            var files =
                (from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "*.lnk", SearchOption.AllDirectories)
                 select Path.GetFileName(file)).ToList(); //TOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLIST

            // In case it's a shared computer, get entries from the System's start menu too
            files = files.Concat(
                (from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*.lnk", SearchOption.AllDirectories)
                 where !files.Contains(Path.GetFileName(file))
                 select Path.GetFileName(file)).ToList() //TOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLIST
                 ).ToList(); //TOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLISTTOLIST
                             // ToList() actually prevents a StackOverflowException, because well, otherwise an IEnumerable is looping over itself, and ToList() static-izes it or something

            #endregion

            #region Check shortcuts list

            if (files.Contains("Roblox Player.lnk"))
            {
                spawnAd("whats up with roblox my man?");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("best not be playing transfurmation games");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("get some class, play the infinite ikea game");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("League of Legends.lnk"))
            {
                spawnAd("whats poppin league player?");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("im not gonna say anything else cause i dont want you to smash your controllers in a rage");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("Valorant.lnk")) // im not installing Tencent spyware on my pc for a joke, so this lnk name is a guess
            {
                spawnAd("ayo valorant player");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("hows the rootkit my guy");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("so what do you think about Winnie the Pooh");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("what about Tian-");
                yield return new WaitForSecondsRealtime(1f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("Overwatch.lnk")) // also a guess, but i dont think i have the space to spare for OW
            {
                spawnAd("i see you got overwatch installed");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("hows the meta");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("or balance in general");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("i hear blizzard's not great at that");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("or dealing with public backlash");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("like the time they had the whole hong kong thing");
                yield return new WaitForSecondsRealtime(7f);
                spawnAd("totally unrelated, what do you think about Winnie the Pooh");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("or what about Tian-");
                yield return new WaitForSecondsRealtime(1f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("Genshin Impact.lnk"))
            {
                spawnAd("ayo bruh what up wit the genshit simpact doe?");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("you a 'shes 3000 years old in the lore' type of mf huh");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("or maybe you like grinding for hours at a time, or you like being separated from your, or your parents', hard earned dollars");
                yield return new WaitForSecondsRealtime(6f);
                spawnAd("a sucker is born every minute, as they say");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("osu!.lnk"))
            {
                spawnAd("osu player headass, tell me whats it like to have your ears blasted by anime OSTs and nightcore");
                yield return new WaitForSecondsRealtime(6f);
                spawnAd("s'pose i could say the same about beat saber, but it's not as.... prevalent, in the BS community");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (files.Contains("Ultimaker Cura 4.10.0.lnk")) // fuck, i need to update this when it updates, WHY ULTIMAKER WHY
            {
                spawnAd("oh shit you use cura?");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("what printer?");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("what filament?");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("what nozzle size?");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("what print speed?");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("send a benchy pic?");
                yield return new WaitForSecondsRealtime(6f);
                spawnAd("please actually send a benchy pic, im extraes on the bw server", false); //set wascaught to false because all sins are forgiven if you have cura :^)
                yield return new WaitForSecondsRealtime(8f);
            }

            #endregion
            
            #region Check for things in game
            
            if ((from ab in GameObject.FindObjectsOfType<StressLevelZero.AI.AIBrain>()
                 select ab.name).Contains("protogen"))
            {
                spawnAd("man you got the mf protogen npc?");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("i oughta hit up a memory leak rn on god");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("i dont even have any other 'ewww furryyyy' quips cause as far as things go, a protogen npc isnt that bad");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("im just putting the protogen npc in this effect because thats like");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("a furry thing");
                yield return new WaitForSecondsRealtime(3f);
                spawnAd("and im putting furry things on the list");
                yield return new WaitForSecondsRealtime(6f);
                spawnAd("hey, if weeb shit gets on the list, then furry shit gets put on the list");
                yield return new WaitForSecondsRealtime(8f);
            }
            #endregion

            if (!BWChaos.isSteamVer)
            {
                spawnAd("alright im done");
                yield break; // Stop here if this isn't the steam version.
            }
            userID = SteamUser.GetSteamID();
            bool ownsHuniePop = false;
            bool ownsAmorous = false;

            #region Generic SteamApps boolean checks

            if (SteamApps.BIsVACBanned())
            {
                spawnAd("howd you get vac banned?");
                yield return new WaitForSecondsRealtime(3.5f);
                spawnAd("downloading skill?");
                yield return new WaitForSecondsRealtime(5f);
            }

            if (SteamApps.BIsSubscribedFromFamilySharing())
            {
                spawnAd("dont even own the game yourself?");
                yield return new WaitForSecondsRealtime(3.5f);
                spawnAd("your homeboy " + SteamFriends.GetFriendPersonaName(SteamApps.GetAppOwner()) + " really coming through huh");
                yield return new WaitForSecondsRealtime(3.5f);
                spawnAd("i guess it beats piracy");
                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion

            #region Check for ( ͡° ͜ʖ ͡°) games

            // huniepop
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)339800)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                ownsHuniePop = true;
                spawnAd("ah yes. huniepop. candy crush for the horny.");
                yield return new WaitForSecondsRealtime(5f);
            }

            // huniepop2
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)930210)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                if (ownsHuniePop)
                {
                    spawnAd("hold up, you got BOTH huniepops?");
                    yield return new WaitForSecondsRealtime(1.5f);
                    spawnAd("i IMPLORE you to take off the headset and talk to a woman");
                }
                else spawnAd("mhm. huniepop 2, the best sequel since white bread followed up wheat bread. for sure.");
                yield return new WaitForSecondsRealtime(5f);
            }

            // vr kanojo
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)751440)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                spawnAd("BOY WHAT THE HELL BOY GETCHO ASS UP ON OUTTA HERE WITCHO VR KANOJO PLAYIN ASS GTFO OUTTA HERE MAAAAAN");
                yield return new WaitForSecondsRealtime(3.5f);
                spawnAd("MAN IVE SEEN SOME DEGENERATES IN MY TIME BUT YOU TOP THE CHARTS");
                yield return new WaitForSecondsRealtime(2.5f);
                spawnAd("NO BITCHES");
                yield return new WaitForSecondsRealtime(0.5f);
                spawnAd("NO HOES");
                yield return new WaitForSecondsRealtime(0.5f);
                spawnAd("NO FEMALE ATTENTION OF ANY KIND");
                yield return new WaitForSecondsRealtime(1.5f);
                spawnAd("TYPE");
                yield return new WaitForSecondsRealtime(0.5f);
                spawnAd("OR VARIETY");
                yield return new WaitForSecondsRealtime(3.5f);
                spawnAd("TOUCH GRASS");
                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion

            #region Check for furry games

            // amorous
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)778700)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                ownsAmorous = true;
                spawnAd("i see youre a furry...");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("so how was uhhh.... ");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("....... ");
                yield return new WaitForSecondsRealtime(6f);
                spawnAd("how was amorous");
                yield return new WaitForSecondsRealtime(2f);
                spawnAd("or how about the feeling of grass");
                yield return new WaitForSecondsRealtime(4f);
                spawnAd("please experience it");
                yield return new WaitForSecondsRealtime(8f);
            }

            // changed
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)814540)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                if (ownsAmorous)
                {
                    spawnAd("so you own amorous, and changed.");
                    yield return new WaitForSecondsRealtime(4f);
                    spawnAd("im just gonna take the liberty of opening a few tabs in your browser real quick");
                    // use DuckDuckGo's bangs because i dont want to paste an amazon URL that could doxx me lol
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+grass");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+indoor+grass");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+indoor+plant");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+succulent+plant");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=how+to+stop+being+a+degenerate"); // use ddg because i dont want google fucking with peoples shit thinking they searched it
                    yield return new WaitForSecondsRealtime(6f);
                    spawnAd("please take this advice to heart");
                    yield return new WaitForSecondsRealtime(2.5f);
                    spawnAd("owning a porn game and a fetish game\nfatherless behavior");
                }
                spawnAd("let me guess, 'changed isnt a fetish game, it actually has good gameplay and an engaging story'");
                yield return new WaitForSecondsRealtime(2.5f);
                spawnAd("uh huh, yeah, sure, kid, tell it to the judge");
                yield return new WaitForSecondsRealtime(2.5f);
                // Open steam overlay to judge image
                SteamFriends.ActivateGameOverlayToWebPage("https://www.inquirer.com/resizer/UILgwAwPDUWh5sfpw4oNaWL37cc=/1400x932/smart/arc-anglerfish-arc2-prod-pmn.s3.amazonaws.com/public/K4OJHGLTGJHGFJQYDNAUFESFWQ.jpg",
                    EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);

                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion


            if (!wasCaught)
            {
                spawnAd("wait...");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("WHAT THE FUCK?????? YOU'RE ACTUALLY NOT A DEGENERATE??????\nHOW????");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("I PUT LIKE 20 CHECKS FOR PORN GAMES AND SHIT, AND YOU DODGED THEM ALL");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("EVEN SHIT LIKE GENSHIN IMPACT AND OSU");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("YOU MADE THIS EFFECT LOOK LIKE A BITCH");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("GOD DAMN!!!!!!");
                yield return new WaitForSecondsRealtime(5f);
                spawnAd("IM HOLDING THIS L IN HONOR OF YOUR SUBSTANTIAL W!!!");
            }
            else spawnAd("alright im done");

        }

        private GameObject spawnAd(string text, bool caught_pleaseignore = true)
        {
            wasCaught = caught_pleaseignore;
            var ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(text);
            var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
            ad.transform.position = phead.position + phead.forward.normalized;
            ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.position);
            return ad;
        }
    }
}
