using Steamworks;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Wait = UnityEngine.WaitForSecondsRealtime;

namespace BLChaos.Effects;

internal class CaughtIn4K : EffectBase
{
    // used to be "Caught in 4K UHD Dolby HDR10 H.265 HEVC With Dolby Atmos Surround Sound for Headphones" but that was stupidly long
    public CaughtIn4K() : base("Caught in 4K", 120) { }

    bool wasCaught = false;
    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        yield return null;
        SpawnAd("let's see here...", false);
        yield return new Wait(5f);

        if (Directory.GetCurrentDirectory().Contains("r2modman"))
        {
            SpawnAd("I see you're using r2modman...");
            yield return new Wait(6f);
            SpawnAd("not that you asked, but once i was against r2modman");
            yield return new Wait(4f);
            SpawnAd("it took some control out of the hands of mod makers");
            yield return new Wait(4f);
            SpawnAd("and it uses electron");
            yield return new Wait(4f);
            SpawnAd("which is basically the entirety of google chrome");
            yield return new Wait(4f);
            SpawnAd("except one program uses the whole of it for one thing");
            yield return new Wait(4f);
            SpawnAd("but now i dont really care because theres less 'how do i extract a 7z' questions", false);
            yield return new Wait(8f);
        }

        #region Read start menu shortcuts

        // Get entries from User's start menu
        System.Collections.Generic.List<string> shortcuts =
            (from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "*.lnk", SearchOption.AllDirectories)
             select Path.GetFileName(file)).ToList();

        // In case it's a shared computer, get entries from the System's start menu too
        shortcuts.AddRange(
            from file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*.lnk", SearchOption.AllDirectories)
            where !shortcuts.Contains(Path.GetFileName(file))
            select Path.GetFileName(file));

        #endregion

        #region Check shortcuts list

        if (shortcuts.Any(f => f.Contains("Cura")))
        {
            SpawnAd("oh shit you use cura?");
            yield return new Wait(4f);
            SpawnAd("what printer?");
            yield return new Wait(2f);
            SpawnAd("what filament?");
            yield return new Wait(2f);
            SpawnAd("what nozzle size?");
            yield return new Wait(2f);
            SpawnAd("what print speed?");
            yield return new Wait(2f);
            SpawnAd("send a benchy pic?");
            yield return new Wait(6f);
            SpawnAd("please actually send a benchy pic, im extraes on the BL server", false); //set wascaught to false because all sins are forgiven if you have cura :^)
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("Roblox Player.lnk"))
        {
            SpawnAd("whats up with roblox my man?");
            yield return new Wait(4f);
            SpawnAd("best not be playing transfurmation games");
            yield return new Wait(5f);
            SpawnAd("get some class, play the infinite ikea game");
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("League of Legends.lnk"))
        {
            SpawnAd("whats poppin league player?");
            yield return new Wait(3f);
            SpawnAd("im not gonna say anything else cause i dont want you to smash your controllers in a rage");
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("Valorant.lnk")) // im not installing Tencent spyware on my pc for a joke, so this lnk name is a guess
        {
            SpawnAd("ayo valorant player");
            yield return new Wait(3f);
            SpawnAd("hows the rootkit my guy");
            yield return new Wait(4f);
            SpawnAd("so what do you think about Winnie the Pooh");
            yield return new Wait(5f);
            SpawnAd("what about Tian-");
            yield return new Wait(1f);
            SpawnAd("");
            yield return new Wait(2f);
            SpawnAd("");
            yield return new Wait(3f);
            SpawnAd("");
            yield return new Wait(4f);
            SpawnAd("");
            yield return new Wait(5f);
            SpawnAd("");
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("Overwatch.lnk")) // also a guess, but i dont think i have the space to spare for OW
        {
            SpawnAd("i see you got overwatch installed");
            yield return new Wait(5f);
            SpawnAd("hows the meta");
            yield return new Wait(4f);
            SpawnAd("or balance in general");
            yield return new Wait(4f);
            SpawnAd("i hear blizzard's not great at that");
            yield return new Wait(5f);
            SpawnAd("or dealing with public backlash");
            yield return new Wait(4f);
            SpawnAd("like the time they had the whole hong kong thing");
            yield return new Wait(7f);
            SpawnAd("totally unrelated, what do you think about Winnie the Pooh");
            yield return new Wait(5f);
            SpawnAd("or what about Tian-");
            yield return new Wait(1f);
            SpawnAd("");
            yield return new Wait(2f);
            SpawnAd("");
            yield return new Wait(3f);
            SpawnAd("");
            yield return new Wait(4f);
            SpawnAd("");
            yield return new Wait(5f);
            SpawnAd("");
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("Genshin Impact.lnk"))
        {
            SpawnAd("ayo bruh what up wit the genshit simpact doe?");
            yield return new Wait(4f);
            SpawnAd("you a 'shes 3000 years old in the lore' type of mf huh");
            yield return new Wait(5f);
            SpawnAd("or maybe you like grinding for hours at a time, or you like being separated from your, or your parents', hard earned dollars");
            yield return new Wait(6f);
            SpawnAd("a sucker is born every minute, as they say");
            yield return new Wait(8f);
        }

        if (shortcuts.Contains("osu!.lnk"))
        {
            SpawnAd("osu player headass, tell me whats it like to have your ears blasted by anime OSTs and nightcore");
            yield return new Wait(6f);
            SpawnAd("s'pose i could say the same about beat saber, but it's not as.... prevalent, in the BS community");
            yield return new Wait(8f);
        }

        #endregion

        #region Check for things in game

        // protogen npc
        if (Utilities.FindAll<SLZ.AI.AIBrain>().Any(ab => ab.name.ToLower().Contains("protogen")))
        {
            SpawnAd("man you got the mf protogen npc?");
            yield return new Wait(4f);
            SpawnAd("i oughta hit up a memory leak rn on god");
            yield return new Wait(4f);
            SpawnAd("i dont even have any other 'ewww furryyyy' quips cause as far as things go, a protogen npc isnt that bad");
            yield return new Wait(4f);
            SpawnAd("im just putting the protogen npc in this effect because thats like");
            yield return new Wait(3f);
            SpawnAd("a furry thing");
            yield return new Wait(3f);
            SpawnAd("and im putting furry things on the list");
            yield return new Wait(6f);
            SpawnAd("hey, if weeb shit gets on the list, then furry shit gets put on the list");
            yield return new Wait(8f);
        }

        // femboy npc for the fatherless
        if (Utilities.FindAll<SLZ.AI.AIBrain>().Any(ab => ab.name.ToLower().Contains("femb")))
        {
            SpawnAd("please touch grass");
            yield return new Wait(3f);
            SpawnAd("you have a fuckin femboy npc fr bro");
            yield return new Wait(3f);
            SpawnAd("i swear im about to hit up the windows kernel to raise a fuckin exception right now");
            yield return new Wait(5f);
            SpawnAd("that means cause a BSOD.");
            yield return new Wait(6f);
            SpawnAd("no but seriously why the fuck do you have a femboy npc");
            yield return new Wait(4f);
            SpawnAd("did your father not beat you or something");
            yield return new Wait(4f);
            SpawnAd("or did your mother not give you enough attention as a child");
            yield return new Wait(5f);
            SpawnAd("its never too late to go outside and touch grass");
            yield return new Wait(5f);
            SpawnAd("im tempted to freeze the game for ~5 minutes just so you have some time to go outside and acquaint yourself with your lawn");
            yield return new Wait(8f);
        }

        // detect if paranoia is running
        if (AppDomain.CurrentDomain.GetAssemblies().Any(asm => asm.FullName.ToLower().Contains("paranoia")))
        {
            SpawnAd("i see you have paranoia");
            yield return new Wait(5f);
            SpawnAd("dont let him grab you");
            yield return new Wait(5f);
            SpawnAd("DONT LET HIM GRAB YOU.");
            yield return new Wait(2.5f);
            for (int i = 0; i < 5; i++)
            {
                SpawnAd("RUN.");
                yield return new Wait(1f);
                SpawnAd("HE IS COMING.");
                yield return new Wait(1f);
            }

            for (int i = 0; i < 10; i++)
            {
                SpawnAd("IMCOMEYOU.");
                yield return new Wait(1f);
            }

            for (int i = 0; i < 10; i++)
            {
                GameObject sign = SpawnAd("I am in your walls.");
                yield return new Wait(0.5f);
                sign.Destroy();
                yield return new Wait(0.5f);
            }
        }

        #endregion

        #region System info checks (thanks Unity!)

        if (SystemInfo.batteryLevel != -1)
        {
            SpawnAd("cmon man why the gaming laptop");
            yield return new Wait(4f);
            SpawnAd("those things are like triple the price");
            yield return new Wait(4f);
            SpawnAd("for a system with a weaker gpu");
            yield return new Wait(4f);
            SpawnAd("and 2 minutes of battery life");
            yield return new Wait(6f);
            SpawnAd("the only thing i can credit gaming laptops for");
            yield return new Wait(5f);
            SpawnAd("is a realistic flight simulator experience");
            yield return new Wait(6f);
            SpawnAd("because yknow");
            yield return new Wait(4f);
            SpawnAd("loud ass fans");
            yield return new Wait(8f);
        }

        if (SystemInfo.graphicsDeviceVendorID != 0x10de)
        {
            SpawnAd("lol get a load of the amd gpu user over here");
            yield return new Wait(4f);
            SpawnAd("...or youre using integrated graphics...");
            yield return new Wait(4f);
            SpawnAd("...");
            yield return new Wait(4f);
            SpawnAd("all i know is that youre sure as hell not using an nvidia gpu");
            yield return new Wait(6f);
            SpawnAd("big L to be honest");
            yield return new Wait(8f);
        }

        if (!SystemInfo.processorType.Contains("AMD"))
        {
            SpawnAd("why are you using intel in 2022");
            yield return new Wait(4f);
            SpawnAd("cmon man");
            yield return new Wait(4f);
            SpawnAd("i shouldnt be putting this much logic in the mod to be honest");
            yield return new Wait(6f);
            SpawnAd("cause i dont really want your cpu overheating and blowing up");
            yield return new Wait(6f);
            SpawnAd("large L if i must be serious");
            yield return new Wait(8f);
        }

        #endregion

        SpawnAd("alright im done");
        ForceEnd();
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        string text = Encoding.ASCII.GetString(data[1]);
        GameObject ad = Utilities.SpawnAd(text);
        ad.transform.DeserializePosRot(data[0]);
    }

    private GameObject SpawnAd(string text, bool __caught_pleaseignore = true)
    {
        wasCaught = __caught_pleaseignore;
        // this literally just creates an ad and puts it in front of the player. code is not pretty sometimes.
        GameObject ad = BoneLib.RandomShit.PopupBoxManager.CreateNewPopupBox(text);
        Transform phead = GlobalVariables.Player_PhysRig.torso.rbHead.transform;
        ad.transform.position = phead.position + phead.forward.normalized;
        ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.position);

        SendNetworkData(ad.transform.SerializePosRot(), Encoding.ASCII.GetBytes(text));
        return ad;
    }
}
