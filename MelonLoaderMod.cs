using BW_Chaos.Effects;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WatsonWebsocket;

namespace BW_Chaos
{
    public static class BuildInfo
    {
        public const string Name = "BW_Chaos"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "extraes"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.1.6"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)        
    }

    public class BW_Chaos : MelonMod
    {
        #region ML Preferences
        internal string token = "YOUR_TOKEN_HERE";
        internal string channelPref = "CHANNEL_ID_HERE";
        internal string nodePref = @"C:\Program Files\nodejs\node.exe";
        internal bool randomOnNoVotes = false;
        internal bool boneMenuEntries = false;
        internal bool steamFuckery = true;
        internal bool useGravityEffects = true;
        internal bool useLaggyEffects = true;
        #endregion
        public override void OnApplicationStart()
        {
            if (token == null)
            {
                MelonLogger.Msg("Hello DNSpy user, let me regale you with a tale from my youth");
                MelonLogger.Msg("https://hastebin.com/udoveyazoy.pl");
                MelonLogger.Msg("");
                MelonLogger.Msg("");
                MelonLogger.Msg("Since you're here, if you don't know already, the next major version will be the rewritten version done in collaboration with TrevTV, so it should be less stupidly made.");
                MelonLogger.Msg("No but for real, this mod is closed source for a reason. Distributing this mod without prior permission of me, extraes, is messed up.");
                MelonLogger.Msg("Distributing a version of this mod with slight alterations is also messed up, and you shouldn't do it.");
                MelonLogger.Msg("This is my first serious mod project, and this build is the culmination of a lot of effort and four weeks of learning C#.");
                MelonLogger.Msg("If you want to look at some of my code, tell me and I might give you some of the source, because as it is, DNSpy shows some hokey shit for some of my functions");
            }

            #region Get and set values from MelonPrefs
            // pragmas here because ML decides to obsolete shit for no fucking reason, like MelonLogger.Log and now MelonPreferences.CreateCategory and MelonPreferences.CreateEntry
#pragma warning disable CS0612 // Type or member is obsolete
            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");
            MelonPreferences.CreateEntry("BW_Chaos", "token", token, "token", false);
            token = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");
            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelPref, "channel", false);
            channelPref = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");
            MelonPreferences.CreateEntry("BW_Chaos", "nodePath", nodePref, "nodePath", false);
            nodePref = MelonPreferences.GetEntryValue<string>("BW_Chaos", "nodePath");
            MelonPreferences.CreateEntry("BW_Chaos", "randomEffectOnNoVotes", randomOnNoVotes, "randomEffectOnNoVotes", false);
            randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");
            MelonPreferences.CreateEntry("BW_Chaos", "enterEffectsIntoBonemenu", boneMenuEntries, "enterEffectsIntoBonemenu", false);
            boneMenuEntries = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enterEffectsIntoBonemenu");
            MelonPreferences.CreateEntry("BW_Chaos", "messWithSteamProfile", steamFuckery, "messWithSteamProfile", false);
            steamFuckery = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "messWithSteamProfile");
            MelonPreferences.CreateEntry("BW_Chaos", "useGravityEffects", useGravityEffects, "useGravityEffects", false);
            useGravityEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useGravityEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useLaggyEffects", useLaggyEffects, "useLaggyEffects", false);
            useLaggyEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useLaggyEffects");
            MelonPreferences.Save(); // BECAUSE IF I DONT SAVE IT RN THEN THE FAT BASTARD WONT CHANGE IT UNTIL THE GAME CLOSES
#pragma warning restore CS0612 // Type or member is obsolete
            #endregion

            if (token != "YOUR_TOKEN_HERE" || channelPref != "CHANNEL_ID_HERE")
            {
                MelonLogger.Msg("Token and channel fetched!");
                UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<STOPEFFECTSFORFUCKSSAKE>();
                MainAsync().GetAwaiter().GetResult();
            }
            else
            {
                MelonLogger.Msg("Welcome to BW Chaos!");
                MelonLogger.Msg("This mod will remain inactive until you place a Discord BOT token and channel ID into melonprefs.");
                MelonLogger.Msg("If you have Node.js installed and at a different location than" + nodePref + ", edit the entry in MelonPrefs with the location of node.exe.");
                MelonLogger.Msg("If you want to play the campaign, you can disable effects that change gravity by changing useGravityEffects in melonprefs.\n" +
                                "(This is because if you change gravity, you update every rigidbody, which Unity does not like)");
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.RightControl)) GameObject.FindObjectOfType<Data_Manager>().RELOADSCENE();
            if (Input.GetKeyDown(KeyCode.L))
            {
                string es = "";
                foreach (var e in EffectList) es += ("\n" + e.Name);
                MelonLogger.Msg(es);
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            ModThatIsNotMod.Player.GetRigManager().AddComponent<STOPEFFECTSFORFUCKSSAKE>();

        }

        #region Effect lists declaration
        public int EffectNumberOffset = 0;
        static public List<IChaosEffect> EffectList = new List<IChaosEffect> { };
        public List<IChaosEffect> candidateEffects = new List<IChaosEffect> { };
        static public List<string> ActiveEffects = new List<string> { };
        public IChaosEffect RandomEffect;
        bool GUIEnabled = false;
        float timeSinceEnabled = 0f;
        #endregion
        public override async void OnGUI()
        {
            // Load chaos bars into onscreen UI
            if (GUIEnabled)
            {
                float TimeSinceReset = Time.realtimeSinceStartup - timeSinceEnabled;
                if (TimeSinceReset > 30f)
                {
                    GUI.Box(new Rect(Screen.width * (TimeSinceReset % 2) * 0.5f, 25, 50, 25), "");
                    if (TimeSinceReset > 40f)
                    {
                        if (token != "YOUR_TOKEN_HERE" && channelPref != "CHANNEL_ID_HERE")
                        {
                            GUI.Box(new Rect(Screen.width - 550, 100, 500, 25), "It looks like the mod is spinning idly. This is likely a result of the bot crashing.");
                            GUI.Box(new Rect(Screen.width - 550, 125, 500, 25), "Restart BW & ping '<@261631460727980033>' (extraes) in the BW server.");
                            if (TimeSinceReset < 40.1f) MelonLogger.Error("Mod is spinning, ping extraes in the bw server '<@261631460727980033>'");
                        }
                        else GUI.Box(new Rect(Screen.width - 600, 125, 600, 25), "Fill out the token and channel ID in MelonPrefs!");
                    }
                }
                else
                {
                    int EffectNumber = 0;
                    foreach (IChaosEffect effect in candidateEffects)
                    {
                        GUI.Box(new Rect(50, 50 + (EffectNumber * 25), 500, 25), $"{EffectNumber + EffectNumberOffset + 1}: {effect.Name}");
                        EffectNumber++;
                    }
                    GUI.Box(new Rect(50, 50 + (EffectNumber * 25), 500, 25), $"{EffectNumber + EffectNumberOffset + 1}: Random effect");
                    GUI.Box(new Rect(50, 250, 500, (ActiveEffects.Count + 1) * 15 + 10), "Active effects:\n" + string.Join("\n", ActiveEffects));
                    GUI.Box(new Rect(Screen.width - 550, 50, 500, 25), "Time");
                    GUI.Box(new Rect(Screen.width - 550, 75, 500 * Math.Min(TimeSinceReset % 30 / 30, 1f), 25), "");
                }
            }
            else
            {
                if (Time.realtimeSinceStartup > 35) GUI.Box(new Rect(50, 70, 350, 25), "If you're seeing this, something went wrong...\nNode didn't start or crashed early in startup.\nRestart the game.");
                else
                {
                    GUI.Box(new Rect(50, 25, 350, 25), "BW Chaos: Waiting for start at 32 seconds - " + Time.realtimeSinceStartup);
                    //todo: remove this on release
                    GUI.Box(new Rect(50, 50, 350, 25), "Hawaii Build - By extraes");
                }
            }
            await Task.Delay(0);
        }

        WatsonWsClient client = new WatsonWsClient("localhost", 8999, false);
        private async Task MainAsync()
        {
            #region Check for and install Node
            {
                // D#+ doesn't run in Mono/Unity, so I need to run a nodejs process and read its stdout
                // This is the third time rewriting this line. The pkg'd exe with my transpiled js doesn't
                // acknowledge any events (message or otherwise) and never gets client data (client.user)
                bool HasNode = false;
                try
                {
                    Process.Start(nodePref).Kill();
                    HasNode = true;
                }
                catch
                { }
                if (!HasNode)
                {
                    try
                    {
                        Process.Start("node").Kill();
                        HasNode = true;
                        nodePref = "node";
                    }
                    catch
                    { }
                }

                if (HasNode) MelonLogger.Msg("Node.js is installed, reading and unzipping the bot!");
                else
                {
                    MelonLogger.Msg("Node.js isn't installed! Downloading and installing now!");

                    // Download the file
                    var webclient = new WebClient();
                    string InstallerPath = Path.ChangeExtension(Path.GetTempFileName(), "msi");
                    if (File.Exists(InstallerPath)) File.Delete(InstallerPath);
                    webclient.DownloadFile("https://nodejs.org/dist/v16.3.0/node-v16.3.0-x64.msi", InstallerPath);
                    webclient.Dispose();
                    MelonLogger.Msg("Downloaded the Node v16.3.0 installer to temporary path " + InstallerPath);

                    // Run the installer
                    Process.Start(InstallerPath, " /passive /qb ").WaitForExit();
                    MelonLogger.Msg("Node is now installed, let's extract and run the bot!");

                }
            }
            #endregion

            #region Read and extract zip
            string folder = Path.Combine(Path.GetTempPath(), "BW-Chaos");
            string zipPath = Path.Combine(folder, "cbot.zip");
            string extractedPath = Path.Combine(folder, "cbot-extracted");
            string nodeModulesPath = Path.Combine(extractedPath, "node_modules");
            string JSPath = Path.Combine(extractedPath, BuildInfo.Version + "main.js"); //todo: if this fails, its because of a version & name mismatch
            if (!(File.Exists(JSPath) && Directory.Exists(nodeModulesPath)))
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                using (Stream stream = Assembly.GetManifestResourceStream("BW_Chaos.cbot.zip"))
                {
                    byte[] data;
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        data = ms.ToArray();
                    }
                    if (File.Exists(zipPath)) File.Delete(zipPath);
                    File.WriteAllBytes(zipPath, data);
                }

                if (Directory.Exists(extractedPath)) Directory.Delete(extractedPath, true);
                Directory.CreateDirectory(extractedPath);
                ZipFile.ExtractToDirectory(zipPath, extractedPath);
            }
            else MelonLogger.Msg("The bot is already unzipped and up to date!");
            #endregion

            #region Populate effect list
            if (useGravityEffects)
            {
                EffectList.Add(new ZeroGravity());
                EffectList.Add(new Fling());
                EffectList.Add(new PointToGo());
                EffectList.Add(new Centrifuge());
                EffectList.Add(new California());
                EffectList.Add(new GravityCube());
                EffectList.Add(new InvertGravity());
            }
            EffectList.Add(new Butterfingers());
            EffectList.Add(new FuckYourMagazine());
            EffectList.Add(new FlingPlayer());
            EffectList.Add(new CreateDogAd());
            EffectList.Add(new PlayerPointToGo());
            EffectList.Add(new PlayerCentrifuge());
            EffectList.Add(new SlowShooting());
            EffectList.Add(new Paralyze());
            EffectList.Add(new Parkinsons());
            EffectList.Add(new NoRegen());
            //EffectList.Add(new FuckYourItem()); //todo: test this //todo: this crashes the game
            //todo: spawning npc's breaks game
            //EffectList.Add(new CrabletRain());
            //EffectList.Add(new JumpThePlayer()); // tentative add, let's see if it works
            EffectList.Add(new SpeedUpTime());
            EffectList.Add(new Immortality());
            EffectList.Add(new GhostWorld());
            if (useLaggyEffects)
            {
                EffectList.Add(new PlayerGravity());
                EffectList.Add(new PlayerInverseGravity());
                EffectList.Add(new Lag());
            }
            //BORKED, no crash
            //EffectList.Add(new WhenNoVTEC());
            //EffectList.Add(new WhenVTEC());
            EffectList.Add(new BarrySteakfries());
            EffectList.Add(new VibeCheck());
            EffectList.Add(new MazdaRX8Moment());
            //EffectList.Add(new NullratBath());
            //EffectList.Add(new AdaptiveResBeLike());
            EffectList.Add(new IndexControllerSimulator());
            EffectList.Add(new ClipPlaneScan());
            EffectList.Add(new Nearsighted());
            EffectList.Add(new LowQuality19200());
            if (steamFuckery) EffectList.Add(new ChangeSteamName()); // hey, you can disable it now!
            EffectList.Add(new RandomTimeScale());
            EffectList.Add(new INSTALLGENTOO());
            //EffectList.Add(new WindowsErrorSound());
            EffectList.Add(new CreateRandomAds());
            //EffectList.Add(new FakeCrash());
            //EffectList.Add(new SlowPunch());
            #endregion

            #region Register effects in BoneMenu (if applicable)
            if (boneMenuEntries)
            {
                var menu = MenuManager.CreateCategory("BW Chaos", Color.grey);
                foreach (var e in EffectList)
                {
                    menu.CreateFunctionElement(e.Name, Color.cyan, new Action(() => { DoEffect(e); }));
                }
            }
            #endregion

            #region Start node, hook websocket
            try { Process.Start(nodePref, $" {JSPath} {token} {channelPref}"); }
            catch (Exception err) { MelonLogger.Warning("Node wasn't found! Did you install it to another folder and not update the path in MelonPreferences.cfg?"); throw err; }
            await Task.Delay(2000);
            client.MessageReceived += MessageRecieved;
            client.ServerConnected += Connected;
            client.ServerDisconnected += Disconnected;
            client.Start();
            #endregion
        }

        public override void BONEWORKS_OnLoadingScreen()
        {
            MelonLogger.Msg("oh god, the game is loading! quick! end the effects!");
            foreach (var e in EffectList)
            {
                if (ActiveEffects.Contains(e.Name))
                {
                    MelonLogger.Msg("forcibly stopping effect " + e.Name + "! hopefully this doesnt break anything");
                    ActiveEffects.Remove(e.Name);
                    e.EffectEnds();
                }
            }
        }

        private void MessageRecieved(object sender, MessageReceivedEventArgs args)
        {
            string Message = Encoding.UTF8.GetString(args.Data);
            if (!Message.StartsWith("*"))
            {
                int[] votes = JsonConvert.DeserializeObject<int[]>(Message);
                MelonLogger.Msg($"Recieved votes: A: {votes[0]}, B: {votes[1]}, C: {votes[2]}, D: {votes[3]}");
                if (GUIEnabled)
                {
                    if (EffectNumberOffset == 0) EffectNumberOffset = 4;
                    else EffectNumberOffset = 0;
                    // Apply effects
                    int totalvotes = 0;
                    int MaxValue = votes[0];
                    for (int i = 0; i < votes.Length; i++)
                    {
                        totalvotes += votes[i];
                        if (MaxValue < votes[i])
                        {
                            MaxValue = votes[i];
                        }
                    };

                    int winnar = 3;
                    if (MaxValue != 0) winnar = GetProportionalWinner(votes);

                    candidateEffects.Add(RandomEffect);
                    var sceneName = SceneManager.GetActiveScene().name;
                    if (MaxValue == 0 && !randomOnNoVotes) MelonLogger.Msg("There were no votes, skipping this round of effects.");
                    else if (sceneName == "loadingScene" || sceneName == "scene_mainMenu") MelonLogger.Warning($"The current scene is {sceneName}, not running effect {candidateEffects[winnar]}");
                    else if (ModThatIsNotMod.Player.GetRigManager()?.GetComponent<STOPEFFECTSFORFUCKSSAKE>() == null)
                        MelonLogger.Warning("There was either no rig manager or no SEFFS MB on the rig manager! This is likely a result of a loading screen being active! So, not running " + candidateEffects[winnar]);
                    else DoEffect(candidateEffects[winnar]);

                }
                // Generate new list of effects
                MelonLogger.Msg("Generating new list of candidate effects from list of " + EffectList.Count + " effects");
                candidateEffects.Clear();
                foreach (int number in NonconflictingRandom(EffectList.Count, 3))
                {
                    candidateEffects.Add(EffectList[number]);
                }
                RandomEffect = EffectList[new System.Random().Next(EffectList.Count)];
                timeSinceEnabled = Time.realtimeSinceStartup;
                GUIEnabled = true;
            }
            else
            {
                MelonLogger.Error("Error message from server: " + Message.TrimStart((char)42)); // removes the starting asterisk *
            }

        }

        private void Connected(object sender, object _)
        {
            MelonLogger.Msg("Connected to server! https://i.kym-cdn.com/photos/images/original/002/073/395/c88.png");
            client.SendAsync("Clock");
        }

        private static void Disconnected(object sender, object _)
        {
            MelonLogger.Warning("Disconnected from server... Did you close the Node window?");
            MelonLogger.Msg("Disconnected from bot, BW Chaos will not function");
        }

        /*private async Task CheckOnServer()
        {
            await Task.Delay(32000);
            MelonLogger.Msg("Joe nuts lol");
            _ = CheckOnServer();
            _ = client.SendAsync("It's me, your favorite client, just checking in to tell you I'm alive", System.Net.WebSockets.WebSocketMessageType.Text);
        }*/

        private async void DoEffect(IChaosEffect effect)
        {
            try
            {
                MelonLogger.Msg("Starting effect " + effect.Name);
                effect.EffectStarts();
                ActiveEffects.Add(effect.Name);
                await Task.Delay(effect.Duration * 1000);
                if (ActiveEffects.Contains(effect.Name))
                {
                    MelonLogger.Msg("Ending effect " + effect.Name);
                    effect.EffectEnds();
                    ActiveEffects.Remove(effect.Name);
                }
                else MelonLogger.Msg("Tried ending " + effect.Name + " but it was removed from the list... what?");
            }
            catch (Exception err)
            {
                MelonLogger.Msg("Error running/ending effect " + effect.Name + ", removing it from list of effects");
                MelonLogger.Error(err);
                EffectList.Remove(effect);
            }
        }

        private static List<int> NonconflictingRandom(int max, int number)
        {
            List<int> listNumbers = new List<int>();
            var rand = new System.Random();
            int num;
            for (int i = 0; i < number; i++)
            {
                do
                {
                    num = rand.Next(0, max);
                } while (listNumbers.Contains(num));
                listNumbers.Add(num);
            }
            if (Math.Abs(-1) == -1)
            {
                MelonLogger.Msg("This area is unreachable, so I'll put attributions in it.");
                MelonLogger.Msg("TrevTV - General C# help.");
                MelonLogger.Msg("TrevTV - MelonPreferences implementation.");
                MelonLogger.Msg("TrevTV - Packaging nodejs and file into an executable.");
                MelonLogger.Msg("Pointing me towards the Unity Scripting Reference and websockets and WatsonWebsocket.");
                MelonLogger.Msg("Embedding executable into dll and read it (I didnt end up needing to embed an executable, but I embedded a zip).");
                MelonLogger.Msg("YOWChap & WNP - Helping me with stupid problems I should have noticed.");
                MelonLogger.Msg("YOWChap - BMTK/MTINM");
                MelonLogger.Msg("Camobiwon - Globalmodifiers source (found out how to modify gravity and time from it).");
                MelonLogger.Msg("Stackoverflow - Generate list of random numbers without repeats (https://stackoverflow.com/questions/30014901/generating-random-numbers-without-repeating-c).");
                MelonLogger.Msg("Various forums - General help.");
                MelonLogger.Msg("This area is unreachable, so I'll put attributions in it.");
            }
            return listNumbers;

        }

        private static int GetProportionalWinner(int[] votes)
        {
            int totalvotes = 0;
            foreach (int vote in votes) totalvotes += vote;

            var ran = new System.Random().Next(0, totalvotes) + 1;
            for (var i = 0; i < votes.Length; i++)
            {
                if (ran - votes[i] <= 0)
                {
                    return i;
                }
                else ran -= votes[i];
            }
            return 0;
        }

        #region Hook punch
        public static event Action<Collision, float, float> OnPunch; //= new Action<Collision, float, float>(;
        /*[HarmonyPatch(typeof(HandSFX), "PunchAttack")]
        public static class PunchPatch
        {
            public static void Prefix(Collision c, float impulse, float relVelSqr)
            {
                OnPunch(c, impulse, relVelSqr);
            }
        }*/
        #endregion

    }

    class STOPEFFECTSFORFUCKSSAKE : MonoBehaviour
    {
        public STOPEFFECTSFORFUCKSSAKE(IntPtr ptr) : base(ptr) { }

        public void OnDestroy()
        {
            MelonLogger.Msg("oh god, the game is loading! quick! end the effects!");
            while (BW_Chaos.ActiveEffects.Count < 0)
            {
                foreach (var e in BW_Chaos.EffectList)
                {
                    if (BW_Chaos.ActiveEffects.Contains(e.Name))
                    {
                        MelonLogger.Msg("forcibly stopping effect " + e.Name + "! hopefully this doesnt break anything");
                        BW_Chaos.ActiveEffects.Remove(e.Name);
                        e.EffectEnds();
                    }
                }
            }
        }
    }

}
/* LIST OF EFFECTS
 * Pause time at random intervals or when gun is shot
 * Change gravity when gun is shot (throw everything into the sky)
 * Spawn the demon cube for 5s/longer with random rotation at level origin
 * Yeet the player
 * Jump the player (spawn 5+ enemies around player)
 * Chance to eject mag on gunshot
 * Cha-Cha slide: BoneworksModdingToolkit.Player.FindPlayer().transform.eulerAngles
 * Create dog ad: ModThatIsNotMod.RandomShit.AdManager.CreateDogAd
 * Super slow guns: ModThatIsNotMod.Extensions.SetRpm
 * Make gravity switch to wall/ceiling at intervals: Physics.gravity = new Vector3(0, 0.5f, 0);
 * An effect that, if it gets picked 5 times exits the game (or crashes pc based on melonprefs entry)
 */

/* ATTRIBUTIONS
 * Hawaii - Being GMT-11, it's still technically friday as of the time of upload, so that's why this is the Hawaii build.
 * GS - Testing the mod with me over several weeks. The tens of crashes you endured weren't in vain (I hope)
 * GS - Suggesting new QOL modpref settings (Changing nodejs path)
 * Cyanide - Testing during the final ~2 weeks of development
 * The homies in chill (Mr. Gaming, D4-LT, KooloEdits, Maranara, Riggle, Parzival, TabloidA, TheDarkElk, L4rs, TrevTV, WNP78) - Helping test the mod by voting on effects
 * TrevTV - General C# help
 * TrevTV - MelonPreferences implementation
 * TrevTV - Packaging nodejs and file into an executable
 * TrevTV - Embedding executable into dll and read it (I didnt end up needing to embed an executable, but I embedded a zip file)
 * TrevTV - Pointing me towards the Unity Scripting Reference and websockets and WatsonWebsocket
 * TrevTV - Rewriting the discord bot in C# (testing builds still use cbot.zip though lol) <- and the first release, apparently
 * TrevTV - Giving me code to hook punching (used in SUPER PUNCH)
 * TrevTV the fucking GOAT - telling me how to attach a monobehaviour to the player and make code run when the player's gameobject is destroyed
 * Lars - & tear, if you get me
 * WNP - Told me to use sqrMagnitude instead of calculating the distance becaust sqrt is slower
 * * Iterating through a long ass list of gameobjects needs to be as fast as possible, so this was important to PlayerGravity and the other effects that use "local gravity"
 * WNP - Telling me about GlobalPool.Spawn
 * WNP & Lars - Telling me that Poolee.Despawn() is broken and to just set a gameobject as inactive
 * WNP & Lars - Telling me about Poolee.Pool.Despawnall() (it may come in handy later lol)
 * YOWChap, Lars, & WNP - Helping me with stupid problems I should have noticed
 * YOWChap - BMTK/MTINM
 * Adamdev - Testing the initial proof of concept
 * Adamdev - Telling me about UnityExplorer (and sending me the DLL)
 * Adamdev - Helping me with FuckYourMagazine and Butterfingers
 * Lakatrazz - Telling me about PhysBody
 * TheShadowNinja - Telling me about AddComponent (add rb to gravity cube)
 * Elarelda - Elareldeffect idea & details
 * Microsoft - C# documentation (duh)
 * FatWrinkleZ - SusArrow, I dnspy'd it to get the code for bootleg gravity cube & pointtogo.
 * C# - If else statements 🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏
 * Stackoverflow - Generate list of random numbers without repeats (https://stackoverflow.com/questions/30014901/generating-random-numbers-without-repeating-c)
 * Stackoverflow - Probably a lot of other things but I don't remember
 * Forums - Change gravity direction, get list of all gameobjects
 * ChaosModV - "Inspiration" for some effects
 */