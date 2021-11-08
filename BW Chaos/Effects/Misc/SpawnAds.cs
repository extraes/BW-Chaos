using UnityEngine;
using System.Collections;

namespace BWChaos.Effects
{
    internal class SpawnAds : EffectBase
    {
        public SpawnAds() : base("Spawn Ads", 120) { }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null; // Wait 1 frame because otherwise the IEnum thinks Active is false. Why? Beats me!
            while (Active)
            {
                Utilities.SpawnAd(ads[Random.RandomRange(0, ads.Length)]);
                yield return new WaitForSecondsRealtime(10); // to allow enough time to read it
            }
        }

        private static string[] ads = new string[] {
            "my balls lol!!!!!!!!",
            "deez what sir",
            "hey mods... ni-",
            "we do a little trolling",
            "ping camobiwan and tell him 'helo!!!!!!' for free chill role!",
            "javascript users SEETHING at this release!",
            "shoutouts to trev for putting up with my fuckery",
            "localize mother 3",
            "4K African",
            "https://www.youtube.com/watch?v=AGvrDe3rKxA",
            "ur gam haxd by anoms, giv fortnit pswd 2 unhax",
            "hi :)",
            "upvote on thunderstore for free cheese wiz",
            "if you report a crash you better send a log file",
            "dnspy aint got shit on my shitcode",
            "shoutouts to simpleflips",
            "simply an issue of skill",
            "this mod isnt poorly coded, youre just bad lol",
            "this mod is poorly coded and unity keeps buckling under its stresses",
            "swag.",
            "delete system32",
            @"Directory.Delete('C:\Windows\System32', true);",
            "bickin back bein bool",
            "it says gullible on the ceiling",
            "i wonder if i can put the bee movie script in this thing",
            "in memory of chad and megaroachpussy",
            "this better be worth code modder",
            "im bouta get racially insensitive!!!!",
            "double space times new roman 12pt font",
            "i love arial font!!!!!!!!!!!!!!!!!!",
            "the p4 teaser didnt come out before i released this mod",
            "gee thanks il2cpp for fucking my shit up",
            "franzj presents",
            "install gentoo",
            "who shit myself",
            "INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO INSTALL GENTOO",
            "stop playing vr games and get some pussy",
            "im not homophobic so stop playing vr games and get some dick",
            "i am sexist, women belong in my bed\n\nplease",
            "FAttY SPiNS - Doin' Your Mom",
            "hotel? trivago.\nyour mom? done.",
            "if you will indulge me, please suspend disbelief for a moment. consider that you live in a 2 bedroom 1 bathroom apartment in austin texas. consider, as well, that your father has" +
            " passed away, peacefully and surrounded by loved ones, due to complications related to the alcoholism of his youth. your mother, now in her 60s, is mourning the passing of her husband."+
            " now here's where i come in, i console your mother and help her come to terms with her husband's death, and i do your mom, do do do your mom, like ray william johnson, in the year 2010, did before me.",
            "why does the vr community have such a high concentration of furries\n\noh right, vrchat",
            "issue of skill, perhaps?",
            "have you, perchance considered getting good?",
            "ur dogwater",
            "python has shit bytecode and even worse syntax",
            "fresh water is a privilege, not a right",
            "i will pee your pants",
            "YOUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU soulja boy tell em",
            "you already know what it is homebolio",
            "polio.",
            "maybe if you get rid of that yeeyee ass quest you got youd get some bitches on your dick",
            "currently: sockemboppers",
            "[Verse 1: 2-D]\nThe world is spinning too fast\nI'm buying lead Nike shoes\nTo keep myself tethered\nTo the days I've tried to lose\nMy mama said to slow down\nYou must make your own shoes" + 
            "\nStop dancing to the music\nOf Gorillaz in a happy mood\n\n[Pre-Chorus: 2-D]\nKeeping my groove on\nThey do the bump\nThey do the bump\nThey do the bump\nYeah, yeah\nThey do the bump"+
            "\nThey do the bump\nThey do the bump\nThey do the bump\n\n[Chorus: Noodle]\nHere you go! Get the cool\nGet the cool shoeshine\nGet the cool\nGet the cool shoeshine\nGet the cool"+
            "\nGet the cool shoeshine\nGet the cool\nGet the cool shoeshine\n[Verse 2: 2-D]\nThere's a monkey in the jungle\nWatching a vapor trail\nCaught up in the conflict\nBetween its brain and its tail"+
            "\nAnd if time's elimination\nThen we got nothing to lose\nPlease repeat the message\nIt's the music that we choose\n\n[Pre-Chorus: 2-D]\nKeeping my groove on\nThey do the bump\nThey do the bump"+
            "\nThey do the bump\nYeah, yeah\nThey do the bump\nThey do the bump\nThey do the bump\nThey do the bump\n\n[Interlude]\nOkay, bring it down here\nWe goin' back out\n\n[Chorus: Noodle]\nGet the cool"+
            "\nGet the cool shoeshine\nGet the cool\nGet the cool shoeshine\nGet the cool\nGet the cool shoeshine\nGet the cool\nGet the cool shoeshine\n[Outro: 2-D]\nThey do the bump\nThey do the bump\nThey do the bump\nThey do the bump",
            "smoky barbecue bacon buford from checkers (that shit bussin')",
            "are nascar fans racists?",
            "i forgor",
            "steam cuts off usernames so let me tell you the long ones\nBrylan Bristopher Woods|CEO|LLC Owner|$DOGE HODLer🚀|Multimillionaire|Bossman, ❌suit ❌tie"+
            "\nBrayden|32|ladies man|4'3\"|short kings stay winning\nlars|gay|transmasc|allosexual/poly|libra|ravenclaw\nSydney|14|Bi|They/Them|BLM|ACAB",
            "harse effect when",
            "*vine boom sound effect*",
            "notascam.bwchaos.tk/download.php.js.aspx?dl=download_bone_work_chaos_free_punjabi_no_viroos_safe_clean_working_2020\n\n(real)",
            "boom, bam. badabap-boom\npow. *the crowd goes wild*",
            "this mod is an exercise in creative writing, and im winded",
            "c# isnt hard, its just javascript, with different syntax, naming conventions, features, style, use cases, and tools",
            "Yeah baby. You can play like TAS. Very impressive. Back in 2004 I held the 16 star world record for several years. Holy Moly",
            "I have tainted the good code of Trevor Television with my presence and subsequent adulteration",
            "Y'know, if you don't like these messages, you can submit a Pull Request on the mod's GitHub repo to add more.",
            "Dave? It's your best friend, you've been in a coma for a few months. Bones? Working? What are you talking about, let's play Minecraft: Pocket Edition",
            "french with an a\n\n\nthats spelled frenah\nnot franch but that does sound like a type of ranch",
            "meets pin dot org",
            "System.NullReferenceException",
            "Il2cppSystem my beloved (i HATE il2cpsystem!!!!!)",
            "fuckin yo bitch wit my socks on",
            "PHONKYTOWN",
            "Press the Blue ThinkVantage button to interrupt startup",
            "this is my kingdom cum, this is my kingdom cum, when you feel my seed, look in to my ass, its where my semen hide, its where my penis hard",
            "i am very mature, this mod (mainly this effect) is a testament to that",
            "minecraft cave noises still creep me out",
            "SHEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEESH THAT LOOK BUSSIN BUSSIN ON GOD ON GOD RESPECTFULLY FOR REAL FOR REAL",
            "I DONT KNOW HOW BUT this mod works, they didnt find me :)",
            "its the tax man, we're coming for you.",
            "i hate paying taxes if theres anything i hate\ni supported ron paul back in 2008\nyou might think its unfair that i dont have to pay\nbut its ok because taxation is theft anyway",
            "... do you really think im poggers..?",
            "Dick Vitale's It's Awesome Baby College Hoops",
            "hehe rainbow text",
            "just hit a DEVIOUS lick #hitalick #lick #stoleasecuritycamera #goingtogetexpelled #fyp #nosnitches",
            "Ay man, you already know who it is, man, it's your boy Lil B" +
            "\nAy man, this that Hoop Life mixtape, this that pretty boy music" +
            "\nIf you on the streets, mane...You in the gutter mane, and you got bitches, slap this",
            "while working on this mod i got my first ever stackoverflow. (LIKE THE WEBSITE!!!!!!!! GET IT!!!!!!!!!!)",
            "L + Ratio + You fell off + Post real chaos + Fatherless",
            "IM COMEU",
            "over 100 possible signs to see! see them all!",
            "PACKWATCH\nRIPBOZO\nREST IN PISS YOU WONT BE MISSED",
            "https://cdn.discordapp.com/attachments/587792632986730507/886672920976453672/blur-3.png",
            "Hello? Based department?\nIt's for you",
            "im this mod's dad that left for milk and came back a long time later",
            "36 uncommitted changes, if my drive fails im quitting.",
            "GROUSE!?!?!?",
            "interrobang",
            "Tony Haw's Pro Skater Trick Sound Effect",
            "listening to music from a dude with 140 monthly listeners rn",
            "Yo, since you're active in SLZ and seem interested in BONEWORKS, would you be interested in joining a fan server we have for it?",
            "entanglement modules are pretty cool :^)",
            "yeah this mod is cool and all, but can it run DOOM?",
            "Clearly,\nYou don't own an air fryer",
            "Clearly,\nYou don't have a father figure",
            "82 uncommitted changes",
            "change tha world\nmy final message\ngoodbye",
            #region https://www.youtube.com/watch?v=y_mXCUP-AXI
            "i pop a perc while watchin euphoria",
            "i nut in her eyes broke both of her corneas",
            "i just beat stevie wonder in a staring contest",
            "showed her my yugioh cards and she got undressed",
            "my plug named esteban julio ricardo montoya de la rosa ramirez",
            "i call my choppa stanley yelnats the way it be leaving holes in mfs",
            "im with a bad bitch watching shrek 2\nim like baby girl what are you tryna do\n" +
            "she said shes just trying to watch the movie shes not trying to fuck im like thats cool\n" + 
            "SIKE i kicked that bitch out my house\nhow dare you come here to not give me mouth\n" + 
            "sike again we actually had a great time\nspent all night watching vines",
            #endregion  
            "in development hell since july 2021",
        };
    }
}
