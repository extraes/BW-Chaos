using BW_Chaos_Effects;
using MelonLoader;
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
using WatsonWebsocket;

namespace BW_Chaos
{
    public static class BuildInfo
    {
        public const string Name = "BW_Chaos"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "extraes"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.1.2"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)        
    }

    public class BW_Chaos : MelonMod
    {
        internal string token = "YOUR_TOKEN_HERE";
        internal string channelPref = "CHANNEL_ID_HERE";
        internal string nodePref = @"C:\Program Files\nodejs\node.exe";

        public override void OnApplicationStart()
        {
            if (token == null)
            {
                MelonLogger.Msg("Hello DNSpy user, let me regale you with a tale from my youth");
                MelonLogger.Msg("https://hastebin.com/udoveyazoy.pl");
                MelonLogger.Msg("");
                MelonLogger.Msg("");
                MelonLogger.Msg("");
                MelonLogger.Msg("No but for real, this mod is closed source for a reason. Distributing this mod without prior permission of me, extraes, is messed up.");
                MelonLogger.Msg("Distributing a version of this mod with slight alterations is also messed up, and you shouldn't do it.");
                MelonLogger.Msg("This is my first serious mod project, and this build is the culmination of a lot of effort and two weeks of learning C#.");
                MelonLogger.Msg("If you want to look at some of my code, tell me and I might give you some of the source, because as it is, DNSpy shows some hokey shit for some of my functions");
            }

            #region Get and set values from MelonPrefs
            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");
            MelonPreferences.CreateEntry("BW_Chaos", "token", token, "token", false);
            token = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");
            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelPref, "channel", false);
            channelPref = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");
            MelonPreferences.CreateEntry("BW_Chaos", "node_path", nodePref, "node_path", false);
            nodePref = MelonPreferences.GetEntryValue<string>("BW_Chaos", "node_path");
            MelonPreferences.Save(); // BECAUSE IF I DONT SAVE IT RN THEN THE FAT BASTARD WONT CHANGE IT UNTIL THE GAME CLOSES
            #endregion

            if (token != "YOUR_TOKEN_HERE" || channelPref != "CHANNEL_ID_HERE")
            {
                MelonLogger.Msg("Token and channel fetched!");

                MainAsync().GetAwaiter().GetResult();
            }
            else
            {
                MelonLogger.Msg("Welcome to BW Chaos!");
                MelonLogger.Msg("This mod will remain inactive until you place a Discord BOT token and channel ID into melonprefs.");
                MelonLogger.Msg("If you have Node.js installed and at a different location than" + @"C:\Program Files\nodejs\node.exe" + ", edit the entry in MelonPrefs with the location of node.exe.");
            }
        }

        #region Effects declaration
        public int EffectNumberOffset = 0;
        public List<IChaosEffect> EffectList = new List<IChaosEffect> { };
        public List<IChaosEffect> CandidateEffects = new List<IChaosEffect> { };
        public List<string> ActiveEffects = new List<string> { };
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
                        if (token != "YOUR_TOKEN_HERE" && channelPref != "CHANNEL_ID_HERE") {
                            GUI.Box(new Rect(Screen.width - 550, 100, 500, 25), "It looks like the mod is spinning idly. This is likely a result of the bot crashing.");
                            GUI.Box(new Rect(Screen.width - 550, 125, 500, 25), "Restart BONEWORKS and ping '<@261631460727980033>' (extraes) in the BW server.");
                            if (TimeSinceReset < 40.5f) MelonLogger.Error("Mod is spinning, ping extraes in the bw server '<@261631460727980033>'");
                        }
                        else GUI.Box(new Rect(Screen.width - 550, 125, 500, 25), "Fill out the token and channel ID in MelonPrefs!");
                    }
                }
                else
                {
                    int EffectNumber = 0;
                    foreach (IChaosEffect effect in CandidateEffects)
                    {
                        GUI.Box(new Rect(50, 50 + (EffectNumber * 25), 500, 25), $"{EffectNumber + EffectNumberOffset + 1}: {effect.Name}");
                        EffectNumber++;
                    }
                    GUI.Box(new Rect(50, 50 + (EffectNumber * 25), 500, 25), $"{EffectNumber + EffectNumberOffset + 1}: Random effect");
                    GUI.Box(new Rect(50, 250, 500, (ActiveEffects.Count + 1) * 20 + 10), "Active effects:\n" + String.Join("\n", ActiveEffects));
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
                    GUI.Box(new Rect(50, 50, 350, 25), "Made by extraes");
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
            string ZipPath = Path.Combine(folder, "cbot.zip");

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            using (Stream stream = Assembly.GetManifestResourceStream("BW_Chaos.cbot.zip"))
            {
                byte[] data;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }
                if (File.Exists(ZipPath)) File.Delete(ZipPath);
                File.WriteAllBytes(ZipPath, data);
            }

            string ExtractedPath = Path.Combine(folder, "cbot-extracted");
            if (Directory.Exists(ExtractedPath)) Directory.Delete(ExtractedPath, true);
            Directory.CreateDirectory(ExtractedPath);
            ZipFile.ExtractToDirectory(ZipPath, ExtractedPath);
            string JSPath = Path.Combine(ExtractedPath, "main.js");
            #endregion

            #region Populate effect list
            if (token == null) MelonLogger.Msg("since youre looking, can you propose a better way to do this? cause this right here looks dumb as fuck.");
            EffectList.Add(new ZeroGravity());
            EffectList.Add(new Fling());
            EffectList.Add(new Butterfingers());
            EffectList.Add(new FuckYourMagazine());
            EffectList.Add(new Lag());
            EffectList.Add(new FlingPlayer());
            EffectList.Add(new CreateDogAd());
            EffectList.Add(new InvertGravity());
            EffectList.Add(new PointToGo());
            EffectList.Add(new PlayerPointToGo());
            EffectList.Add(new Centrifuge());
            EffectList.Add(new California());
            EffectList.Add(new PlayerCentrifuge());
            EffectList.Add(new SlowShooting());
            EffectList.Add(new JetpackJoyride());
            EffectList.Add(new Paralyze());
            EffectList.Add(new BootlegGravityCube());
            EffectList.Add(new Parkinsons());
            EffectList.Add(new NoRegen());
            EffectList.Add(new FuckYourItem());
            EffectList.Add(new CrabletRain());
            EffectList.Add(new SpeedUpTime());
            EffectList.Add(new Immortality());
            EffectList.Add(new Accelerate());
            EffectList.Add(new RandomRigShit());
            EffectList.Add(new JumpThePlayer());
            EffectList.Add(new PlayerGravity());
            #endregion

            #region Start node, hook websocket
            MelonLogger.Msg(nodePref);
            try { Process.Start(nodePref, $" {JSPath} {token} {channelPref}"); }
            catch (Exception err) { MelonLogger.Warning("Node wasn't found! Did you install it to another folder and not update the path in MelonPreferences.cfg?"); throw err; }
            await Task.Delay(2000);
            client.MessageReceived += MessageRecieved;
            client.ServerConnected += Connected;
            client.ServerDisconnected += Disconnected;
            client.Start();
            #endregion
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
                    int MaxIndex = 0;
                    int MaxValue = votes[0];
                    for (int i = 0; i < votes.Length; i++)
                    {
                        if (MaxValue < votes[i])
                        {
                            MaxValue = votes[i];
                            MaxIndex = i;
                        }
                    };
                    CandidateEffects.Add(RandomEffect);
                    if (MaxValue == 0) MelonLogger.Msg("There were no votes, skipping this round of effects.");
                    else DoEffect(CandidateEffects[MaxIndex]);

                }
                // Generate new list of effects
                MelonLogger.Msg("Generating new list of candidate effects from list of " + EffectList.Count + " effects");
                CandidateEffects.Clear();
                foreach (int number in NonconflictingRandom(EffectList.Count, 3))
                {
                    CandidateEffects.Add(EffectList[number]);
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

        private async Task CheckOnServer()
        {
            await Task.Delay(32000);
            MelonLogger.Msg("Joe nuts lol");
            _ = CheckOnServer();
            _ = client.SendAsync("It's me, your favorite client, just checking in to tell you I'm alive", System.Net.WebSockets.WebSocketMessageType.Text);
        }

        private async void DoEffect(IChaosEffect effect)
        {
            try
            {
                MelonLogger.Msg("Starting effect " + effect.Name);
                effect.EffectStarts();
                ActiveEffects.Add(effect.Name);
                await Task.Delay(effect.Duration * 1000);
                MelonLogger.Msg("Ending effect " + effect.Name);
                effect.EffectEnds();
                ActiveEffects.Remove(effect.Name);
            } catch (Exception err)
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
 * TrevTV - General C# help
 * TrevTV - MelonPreferences implementation
 * TrevTV - Packaging nodejs and file into an executable
 * TrevTV - Embedding executable into dll and read it (I didnt end up needing to embed an executable, but I embedded)
 * TrevTV - Pointing me towards the Unity Scripting Reference and websockets and WatsonWebsocket
 * YOWChap & WNP - Helping me with stupid problems I should have noticed
 * YOWChap - BMTK/MTINM
 * * Like seriously, there's no SHOT this mod could be done without MTINM, it's basically a mod of "idk how to do this shit, but mtinm probably has a function for it"
 * Adamdev - Testing
 * Adamdev - Telling me about UnityExplorer (and sending me the DLL)
 * Adamdev - Helping me with FuckYourMagazine and Butterfingers
 * Lakatrazz - Telling me about PhysBody
 * Microsoft - C# documentation (duh)
 * FatWrinkleZ - SusArrow, I dnspy'd it to get the code for bootleg gravity cube.
 * C# - If else statements 🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏🙏
 * Stackoverflow - Generate list of random numbers without repeats (https://stackoverflow.com/questions/30014901/generating-random-numbers-without-repeating-c)
 * Forums - Change gravity direction
 * ChaosModV - I yoinked the tick idea lol
 */