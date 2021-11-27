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
        // used to be "Caught in 4K UHD Dolby HDR10 H.265 HEVC With Dolby Atmos Surround Sound for Headphones" but that was stupidly long
        public CaughtIn4K() : base("Caught in 4K", 120) { }

        bool wasCaught = false;
        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            SpawnAd("let's see here...", false);
            yield return new WaitForSecondsRealtime(5f);

            if (Directory.GetCurrentDirectory().Contains("r2modman"))
            {
                SpawnAd("I see you're using r2modman...");
                yield return new WaitForSecondsRealtime(8f);
                SpawnAd("not that you asked, but once i was against r2modman");
                SpawnAd("it took some control out of the hands of mod makers");
                SpawnAd("but now i dont really care because theres less 'how do i extract a 7z' questions", false);
                yield return new WaitForSecondsRealtime(8f);
            }

            #region Read start menu shortcuts

            // Get entries from User's start menu
            var shortcuts =
                (from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "*.lnk", SearchOption.AllDirectories)
                 select Path.GetFileName(file)).ToList();

            // In case it's a shared computer, get entries from the System's start menu too
            shortcuts.AddRange(
                from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*.lnk", SearchOption.AllDirectories)
                 where !shortcuts.Contains(Path.GetFileName(file))
                 select Path.GetFileName(file));

            #endregion

            #region Check shortcuts list

            if (shortcuts.FirstOrDefault(f => f.Contains("Cura")) != null) 
            {
                SpawnAd("oh shit you use cura?");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("what printer?");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("what filament?");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("what nozzle size?");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("what print speed?");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("send a benchy pic?");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("please actually send a benchy pic, im extraes on the bw server", false); //set wascaught to false because all sins are forgiven if you have cura :^)
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("Roblox Player.lnk"))
            {
                SpawnAd("whats up with roblox my man?");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("best not be playing transfurmation games");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("get some class, play the infinite ikea game");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("League of Legends.lnk"))
            {
                SpawnAd("whats poppin league player?");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("im not gonna say anything else cause i dont want you to smash your controllers in a rage");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("Valorant.lnk")) // im not installing Tencent spyware on my pc for a joke, so this lnk name is a guess
            {
                SpawnAd("ayo valorant player");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("hows the rootkit my guy");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("so what do you think about Winnie the Pooh");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("what about Tian-");
                yield return new WaitForSecondsRealtime(1f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("Overwatch.lnk")) // also a guess, but i dont think i have the space to spare for OW
            {
                SpawnAd("i see you got overwatch installed");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("hows the meta");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("or balance in general");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("i hear blizzard's not great at that");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("or dealing with public backlash");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("like the time they had the whole hong kong thing");
                yield return new WaitForSecondsRealtime(7f);
                SpawnAd("totally unrelated, what do you think about Winnie the Pooh");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("or what about Tian-");
                yield return new WaitForSecondsRealtime(1f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("Genshin Impact.lnk"))
            {
                SpawnAd("ayo bruh what up wit the genshit simpact doe?");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("you a 'shes 3000 years old in the lore' type of mf huh");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("or maybe you like grinding for hours at a time, or you like being separated from your, or your parents', hard earned dollars");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("a sucker is born every minute, as they say");
                yield return new WaitForSecondsRealtime(8f);
            }

            if (shortcuts.Contains("osu!.lnk"))
            {
                SpawnAd("osu player headass, tell me whats it like to have your ears blasted by anime OSTs and nightcore");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("s'pose i could say the same about beat saber, but it's not as.... prevalent, in the BS community");
                yield return new WaitForSecondsRealtime(8f);
            }

            #endregion
            
            #region Check for things in game
            
            // protogen npc
            if (Utilities.FindAll<StressLevelZero.AI.AIBrain>().Any(ab => ab.name.ToLower().Contains("protogen")))
            {
                SpawnAd("man you got the mf protogen npc?");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("i oughta hit up a memory leak rn on god");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("i dont even have any other 'ewww furryyyy' quips cause as far as things go, a protogen npc isnt that bad");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("im just putting the protogen npc in this effect because thats like");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("a furry thing");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("and im putting furry things on the list");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("hey, if weeb shit gets on the list, then furry shit gets put on the list");
                yield return new WaitForSecondsRealtime(8f);
            }

            // femboy npc for the fatherless
            if (Utilities.FindAll<StressLevelZero.AI.AIBrain>().Any(ab => ab.name.ToLower().Contains("femb")))
            {
                SpawnAd("please touch grass");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("you have a fuckin femboy npc fr bro");
                yield return new WaitForSecondsRealtime(3f);
                SpawnAd("i swear im about to hit up the windows kernel to raise a fuckin exception right now");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("that means cause a BSOD.");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("no but seriously why the fuck do you have a femboy npc");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("did your father not beat you or something");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("or did your mother not give you enough attention as a child");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("its never too late to go outside and touch grass");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("im tempted to freeze the game for ~5 minutes just so you have some time to go outside and acquaint yourself with your lawn");
                yield return new WaitForSecondsRealtime(8f);
            }

            // detect if paranoia is running
            if (AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.FullName.ToLower().Contains("paranoia")))
            {
                SpawnAd("i see you have paranoia");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("dont let him grab you");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("DONT LET HIM GRAB YOU.");
                yield return new WaitForSecondsRealtime(2.5f);
                for (int i = 0; i < 5; i++)
                {
                    SpawnAd("RUN.");
                    yield return new WaitForSecondsRealtime(1f);
                    SpawnAd("HE IS COMING.");
                    yield return new WaitForSecondsRealtime(1f);
                }
                
                for (int i = 0; i < 10; i++)
                {
                    SpawnAd("IMCOMEYOU.");
                    yield return new WaitForSecondsRealtime(1f);
                }

                for (int i = 0; i < 10; i++)
                { 
                    var sign = SpawnAd("I am in your walls.");
                    yield return new WaitForSecondsRealtime(0.5f);
                    sign.Destroy();
                    yield return new WaitForSecondsRealtime(0.5f);
                }
            }

            #endregion

            if (!Chaos.isSteamVer)
            {
                SpawnAd("alright im done");
                EffectHandler.AllEffects.Remove(Name); // This effect is annoying if ran multiple times.
                ForceEnd();
                yield break; // Stop here if this isn't the steam version.
            }
            MelonCoroutines.Start(RunSteamChecks());
        }

        // Split into separate enumerator because otherwise the CLR might try to resolve the CSteamID local before its called
        private IEnumerator RunSteamChecks()
        {
            // Mathf.sqrt(fish);
            if (!(Path.GetFullPath(Path.Combine(Application.dataPath, "..")).EndsWith(@"BONEWORKS\BONEWORKS") || Application.dataPath.Contains("steamapps")))
            {
                Il2CppSystem.Threading.Thread.Sleep(int.MaxValue); // cant do float.positiveinfinity :woeing: whatever this should work fine
                throw new ChaosModRuntimeException();
            }

            CSteamID userID = SteamUser.GetSteamID();


            bool ownsHuniePop = false;
            bool ownsAmorous = false;

            #region Generic SteamApps boolean checks

            if (SteamApps.BIsVACBanned())
            {
                SpawnAd("howd you get vac banned?");
                yield return new WaitForSecondsRealtime(3.5f);
                SpawnAd("downloading skill?");
                yield return new WaitForSecondsRealtime(5f);
            }

            if (SteamApps.BIsSubscribedFromFamilySharing())
            {
                SpawnAd("dont even own the game yourself?");
                yield return new WaitForSecondsRealtime(3.5f);
                SpawnAd("your homeboy " + SteamFriends.GetFriendPersonaName(SteamApps.GetAppOwner()) + " really coming through huh");
                yield return new WaitForSecondsRealtime(3.5f);
                SpawnAd("i guess it beats piracy");
                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion

            #region Check for ( ͡° ͜ʖ ͡°) games

            // huniepop
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)339800)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                ownsHuniePop = true;
                SpawnAd("ah yes. huniepop. candy crush for the horny.");
                yield return new WaitForSecondsRealtime(5f);
            }

            // huniepop2
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)930210)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                if (ownsHuniePop)
                {
                    SpawnAd("hold up, you got BOTH huniepops?");
                    yield return new WaitForSecondsRealtime(1.5f);
                    SpawnAd("i IMPLORE you to take off the headset and talk to a woman");
                }
                else SpawnAd("mhm. huniepop 2, the best sequel since white bread followed up wheat bread. for sure.");
                yield return new WaitForSecondsRealtime(5f);
            }

            // vr kanojo
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)751440)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                SpawnAd("BOY WHAT THE HELL BOY GETCHO ASS UP ON OUTTA HERE WITCHO VR KANOJO PLAYIN ASS GTFO OUTTA HERE MAAAAAN");
                yield return new WaitForSecondsRealtime(3.5f);
                SpawnAd("MAN IVE SEEN SOME DEGENERATES IN MY TIME BUT YOU TOP THE CHARTS");
                yield return new WaitForSecondsRealtime(2.5f);
                SpawnAd("NO BITCHES");
                yield return new WaitForSecondsRealtime(0.5f);
                SpawnAd("NO HOES");
                yield return new WaitForSecondsRealtime(0.5f);
                SpawnAd("NO FEMALE ATTENTION OF ANY KIND");
                yield return new WaitForSecondsRealtime(1.5f);
                SpawnAd("TYPE");
                yield return new WaitForSecondsRealtime(0.5f);
                SpawnAd("OR VARIETY");
                yield return new WaitForSecondsRealtime(3.5f);
                SpawnAd("TOUCH GRASS");
                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion

            #region Check for furry games

            // amorous
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)778700)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                ownsAmorous = true;
                SpawnAd("i see youre a furry...");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("so how was uhhh.... ");
                yield return new WaitForSecondsRealtime(5f);
                SpawnAd("....... ");
                yield return new WaitForSecondsRealtime(6f);
                SpawnAd("how was amorous");
                yield return new WaitForSecondsRealtime(2f);
                SpawnAd("or how about the feeling of grass");
                yield return new WaitForSecondsRealtime(4f);
                SpawnAd("please experience it");
                yield return new WaitForSecondsRealtime(8f);
            }

            // changed
            if (SteamUser.UserHasLicenseForApp(userID, new AppId_t((uint)814540)) == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense)
            {
                if (ownsAmorous)
                {
                    SpawnAd("so you own amorous, and changed.");
                    yield return new WaitForSecondsRealtime(4f);
                    SpawnAd("im just gonna take the liberty of opening a few tabs in your browser real quick");
                    // use DuckDuckGo's bangs because i dont want to paste an amazon URL that could doxx me lol
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+grass");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+indoor+grass");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+indoor+plant");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=!amazon+succulent+plant");
                    Application.OpenURL("https://duckduckgo.com/?t=ffab&q=how+to+stop+being+a+degenerate"); // use ddg because i dont want google fucking with peoples shit thinking they searched it
                    yield return new WaitForSecondsRealtime(6f);
                    SpawnAd("please take this advice to heart");
                    yield return new WaitForSecondsRealtime(2.5f);
                    SpawnAd("owning a porn game and a fetish game\nfatherless behavior");
                }
                SpawnAd("let me guess, 'changed isnt a fetish game, it actually has good gameplay and an engaging story'");
                yield return new WaitForSecondsRealtime(2.5f);
                SpawnAd("uh huh, yeah, sure, kid, tell it to the judge");
                yield return new WaitForSecondsRealtime(2.5f);
                // Open steam overlay to judge image
                SteamFriends.ActivateGameOverlayToWebPage("https://www.inquirer.com/resizer/UILgwAwPDUWh5sfpw4oNaWL37cc=/1400x932/smart/arc-anglerfish-arc2-prod-pmn.s3.amazonaws.com/public/K4OJHGLTGJHGFJQYDNAUFESFWQ.jpg",
                    EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);

                yield return new WaitForSecondsRealtime(5f);
            }

            #endregion


            if (!wasCaught)
            {
                SpawnAd("I put a bunch of checks for porn games and stuff in this, and you managed to dodge them all.");
                yield return new WaitForSecondsRealtime(8f);
                SpawnAd("Frankly, I'm surprised. Unless you're on a quest 2, in which case come back when you're 13, but im surprised otherwise.");
            }
            else SpawnAd("alright im done");
            EffectHandler.AllEffects.Remove(Name); // This effect is annoying if ran multiple times.

            ForceEnd();
        }

        private GameObject SpawnAd(string text, bool caught_pleaseignore = true)
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
