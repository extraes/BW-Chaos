using UnityEngine;
using System.Collections;

namespace BWChaos.Effects
{
    internal class SpawnAds : EffectBase
    {
        public SpawnAds() : base("Spawn Ads", 120) { }

        public override void OnEffectStart() => MelonLoader.MelonCoroutines.Start(CoRun());

        private IEnumerator CoRun()
        {
            while (Active)
            {
                var ad = ModThatIsNotMod.RandomShit.AdManager.CreateNewAd(ads[Random.RandomRange(0, ads.Length)]);
                var phead = GlobalVariables.Player_PhysBody.rbHead.transform;
                ad.transform.position = phead.position + phead.forward.normalized;
                ad.transform.rotation = Quaternion.LookRotation(ad.transform.position - phead.position);
                yield return new WaitForSecondsRealtime(10); // to allow enough time to read it
            }
        }

        private string[] ads = new string[] {
            "my balls lol!!!!!!!!",
            "deez what sir",
            "hey mods... ni-",
            "we do a little trolling",
            "ping camobiwan and tell him 'helo!!!!!!' for free chill role!",
            //"you have nodejs installed? ew! a js user!", //todid: changed this in nonshitcode version
            "javascript users SEETHING at this release!",
            "shoutouts to trev for putting up with my fuckery",
            "localize mother 3",
            "4K African",
            "https://www.youtube.com/watch?v=AGvrDe3rKxA",
            "ur gam haxd by anoms, giv fortnit pswd 2 unhax",
            "hi :)",
            "upvote on bonetome for free cheese wiz",
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
            "times new roman 12pt font",
            "i love arial font!!!!!!!!!!!!!!!!!!",
            "the p4 teaser will probably come out before i release this mod",
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
            " now here's where i come in, i console your mother and help her come to terms with her husband's death, and i do your mom, do do do your mom, as ray william johnson, in the year 2010.",
            "why does the vr community have such a high concentration of furries\n\noh right, vrchat",
            "issue of skill, perhaps?",
            "have you, perchance considered getting good?",
            "ur dogwater",
            "python has shit bytecode and even worse syntax",
            "fresh water is a privilege, not a right",
            "i will pee your pants",
            "YOUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU",
            "you already know what it is homebolio",
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
            "smoky barbecue bacon buford from checkers (that shit bussin')",
        };
    }
}
