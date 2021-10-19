using BWChaos.Effects;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WatsonWebsocket;
using static BWChaos.Effects.EffectBase;

namespace BWChaos
{
    public static class BuildInfo
    {
        public const string Name = "BWChaos";
        public const string Author = "extraes, trev";
        public const string Company = null;
        public const string Version = "0.2.0";
        public const string DownloadLink = null;
    }

    public partial class BWChaos : MelonMod
    {
        internal string botToken = "YOUR_TOKEN_HERE";
        internal string channelId = "CHANNEL_ID_HERE";

        internal static bool isSteamVer = !File.Exists(Path.Combine(Application.dataPath, "..", "Boneworks_Oculus_Windows64.exe"));
        internal static bool isTwitch = false;
        internal static bool useLaggyEffects = false;
        internal static bool useGravityEffects = false;
        internal static bool useSteamProfileEffects = false;
        internal static bool showCandidatesOnScreen = true;
        internal static bool sendCandidatesInChannel = true;
        internal static bool ignoreRepeatVotes = false;
        internal static bool proportionalVoting = true;
        internal static bool enableRemoteVoting = false;
        internal static bool syncEffects = false;
        internal static List<EffectBase> asmEffects = new List<EffectBase>();
#if DEBUG
        internal static bool enableIMGUI = false;
#endif

        internal static List<(EffectTypes, bool)> eTypesToPrefs = new List<(EffectTypes, bool)>();
        public static Action<EffectBase> OnEffectRan;

        internal Process botProcess;

        public override void OnApplicationStart()
        {
            #region Check datapath

            if (isSteamVer && !(Path.GetFullPath(Path.Combine(Application.dataPath, "..")).EndsWith(@"BONEWORKS\BONEWORKS") || Application.dataPath.Contains("steamapps")))
                throw new ChaosModStartupException();

            #endregion

            #region MelonPref Setup

            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");
            CreateMelonPreferences();
            GetMelonPreferences();
            MelonPreferences.Save();

            #endregion

            #region Initialize things based off MelonPrefs

            PopulateEffects();
            if (syncEffects) Extras.EntanglementSyncHandler.Init();
            if (enableRemoteVoting)
            {
                // why load them otherwise? theyre just gonna get cleared either way
                botToken = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");
                channelId = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");
                isTwitch = !ulong.TryParse(channelId, out ulong _);
                StartBot();
            }

            #endregion

            #region Load Timer

            MemoryStream memoryStream;
            using (Stream stream = Assembly.GetManifestResourceStream("BWChaos.Resources.chaos_ui_elements"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
            GlobalVariables.WristChaosUI = assetBundle.LoadAsset("Assets/UIStuff/prefabs/ChaosCanvas.prefab").Cast<GameObject>();
            GlobalVariables.WristChaosUI.hideFlags = HideFlags.DontUnloadUnusedAsset;

            GlobalVariables.OverlayChaosUI = assetBundle.LoadAsset("Assets/UIStuff/prefabs/ChaosCanvasOverlay.prefab").Cast<GameObject>();
            GlobalVariables.OverlayChaosUI.hideFlags = HideFlags.DontUnloadUnusedAsset;

            #endregion

            #region Load effect resources

            MelonLogger.Msg("Loading effect resources, please wait...");
            {
                using (Stream stream = Assembly.GetManifestResourceStream("BWChaos.Resources.effectresources"))
                {
                    memoryStream = new MemoryStream((int)stream.Length);
                    using (MemoryStream mStream = new MemoryStream((int)stream.Length))
                    {
                        stream.CopyTo(mStream);
                        GlobalVariables.EffectResources = AssetBundle.LoadFromMemory(mStream.ToArray());
                    }
                }
                GlobalVariables.EffectResources.hideFlags = HideFlags.DontUnloadUnusedAsset; // IL2 BETTER NOT FUCK WITH MY SHIT

#if DEBUG
                MelonLogger.Msg("Loaded effect resources; All resource paths:");
                foreach (var path in GlobalVariables.EffectResources.GetAllAssetNames())
                    MelonLogger.Msg(path);
#endif
            }

            {
                using (Stream stream = Assembly.GetManifestResourceStream("BWChaos.Resources.jevil"))
                {
                    memoryStream = new MemoryStream((int)stream.Length);
                    using (MemoryStream mStream = new MemoryStream((int)stream.Length))
                    {
                        stream.CopyTo(mStream);
                        ModThatIsNotMod.CustomItems.LoadItemsFromBundle(AssetBundle.LoadFromMemory(mStream.ToArray()));
                    }
                }
            }
            MelonLogger.Msg("Done loading effect resources");

            #endregion



#if DEBUG
            MelonLogger.Msg($"Of {Assembly.GetTypes().Where(t => t.BaseType == typeof(EffectBase)).ToArray().Length - 1} total effects, {EffectHandler.AllEffects.Count} are present.");
#endif

            RegisterBoneMenu();
        }

        public override void OnApplicationQuit()
        {
            GlobalVariables.WatsonClient?.Stop();
            botProcess?.Kill();
            botProcess?.Dispose();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // you already know what the fuck goin on
            if (EffectHandler.AllEffects.Count < 1) while (true) { }

            GlobalVariables.Player_BodyVitals =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.BodyVitals>();
            GlobalVariables.Player_RigManager =
                GameObject.FindObjectOfType<StressLevelZero.Rig.RigManager>();
            GlobalVariables.Player_Health =
                GameObject.FindObjectOfType<Player_Health>();
            GlobalVariables.Player_PhysBody =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.PhysBody>();

            new GameObject("ChaosUIEffectHandler").AddComponent<EffectHandler>();
            EffectHandler.advanceTimer = sceneName != "scene_mainMenu";

        }

        public override void OnUpdate()
        {
            foreach (EffectBase effect in GlobalVariables.ActiveEffects)
                effect.OnEffectUpdate();
        }

        public override void OnPreferencesLoaded () => LiveUpdateEffects();

#if DEBUG
        // IMGUI for flatscreen debugging (when the vr no workie :woeis:)
        public override void OnGUI()
        {
            if (enableIMGUI)
            {
                var horizOffset = 5;
                // because otherwise, it clips into unityexplorers top bar lol
                var vertOffset = 25;
                for (int i = 0; i < EffectHandler.AllEffects.Count; i++)
                {
                    if (vertOffset + 30 > Screen.height)
                    {
                        vertOffset = 25;
                        horizOffset += 255;
                    }
                    var e = EffectHandler.AllEffects.Values.ElementAt(i);
                    if (GUI.Button(new Rect(horizOffset, vertOffset, 250, 25), e.Name)) e.Run();
                    vertOffset += 30;
                }
            }
        }
#endif

        #region Websocket Methods

        private async void ClientConnectedToServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Connected to the Discord bot!");
            await GlobalVariables.WatsonClient.SendAsync("ignorerepeatvotes:" + ignoreRepeatVotes);
            // Send data for startup then clear it out so that there's less of an opportunity for reflection to steal shit (i think)
            await GlobalVariables.WatsonClient.SendAsync("token:" + botToken);
            botToken = string.Empty;
            await GlobalVariables.WatsonClient.SendAsync("channel:" + channelId);
            channelId = string.Empty;

        }

        private void ClientDisconnectedFromServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Disconnected from the Discord bot, was the process killed?");
        }

        private void ClientReceiveMessage(object sender, MessageReceivedEventArgs e)
        {
            string[] splitMessage = Encoding.UTF8.GetString(e.Data).Split(':');
            string messageType = splitMessage[0];
            string messageData = string.Join(":", splitMessage?.Skip(1)?.ToArray()) ?? string.Empty;
            switch (messageType)
            {
                case "error":
                    MelonLogger.Error("An error has occured within the Discord bot!");
                    MelonLogger.Error(messageData);
                    break;
                case "log":
                    MelonLogger.Msg(messageData);
                    break;
                default:
                    MelonLogger.Error("UNKNOWN MESSAGE TYPE: " + messageType);
                    break;
            }
        }

        #endregion

        #region Startup Methods

        private void CreateMelonPreferences()
        {
            MelonPreferences.CreateEntry("BW_Chaos", "token", botToken, "token");
            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelId, "channel");
            MelonPreferences.CreateEntry("BW_Chaos", "randomEffectOnNoVotes", EffectHandler.randomOnNoVotes, "randomEffectOnNoVotes");
            MelonPreferences.CreateEntry("BW_Chaos", "useGravityEffects", useGravityEffects, "useGravityEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useSteamProfileEffects", useSteamProfileEffects, "useSteamProfileEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useLaggyEffects", useLaggyEffects, "useLaggyEffects");
            // voteprefs
            MelonPreferences.CreateEntry("BW_Chaos", "showCandidatesOnScreen", showCandidatesOnScreen, "showCandidatesOnScreen");
            MelonPreferences.CreateEntry("BW_Chaos", "sendCandidatesInChannel", sendCandidatesInChannel, "sendCandidatesInChannel");
            MelonPreferences.CreateEntry("BW_Chaos", "ignoreRepeatVotesFromSameUser", ignoreRepeatVotes, "ignoreRepeatVotesFromSameUser");
            MelonPreferences.CreateEntry("BW_Chaos", "proportionalVoting", proportionalVoting, "proportionalVoting");
            MelonPreferences.CreateEntry("BW_Chaos", "enableRemoteVoting", enableRemoteVoting, "enableRemoteVoting");
            // yeah
            MelonPreferences.CreateEntry("BW_Chaos", "syncEffectsViaEntanglement", syncEffects, "syncEffectsViaEntanglement");
#if DEBUG
            MelonPreferences.CreateEntry("BW_Chaos", "enableIMGUI", enableIMGUI, "enableIMGUI");
#endif
        }

        // Make public because there's no harm in it (and because reflection was being a bitch to learn and I didn't want to deal with it
        public static void GetMelonPreferences()
        {
            EffectHandler.randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");
            useGravityEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useGravityEffects");
            useSteamProfileEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects");
            useLaggyEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useLaggyEffects");
            // Voting preferences
            showCandidatesOnScreen = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "showCandidatesOnScreen");
            sendCandidatesInChannel = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "sendCandidatesInChannel");
            ignoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser");
            ignoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser");
            ignoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "proportionalVoting");
            enableRemoteVoting = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enableRemoteVoting");
            // yeah what that comment said
            syncEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "syncEffectsViaEntanglement");
#if DEBUG
            enableIMGUI = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enableIMGUI");
#endif

            eTypesToPrefs.Clear();
            // populate eTypesToPrefs now
            eTypesToPrefs.AddRange(new (EffectTypes, bool)[] {
                (EffectTypes.AFFECT_GRAVITY, useGravityEffects),
                (EffectTypes.AFFECT_STEAM_PROFILE, useSteamProfileEffects),
                (EffectTypes.USE_STEAM, isSteamVer),
                (EffectTypes.LAGGY, useLaggyEffects),
                (EffectTypes.HIDDEN, true),
                (EffectTypes.DONT_SYNC, !syncEffects)
            });
        }

        private void StartBot()
        {
            MelonLogger.Msg("Unpacking and starting discord bot...");

            #region Extract Bot

            string saveFolder = Path.Combine(Path.GetTempPath(), "BW-Chaos");
            string exePath = Path.Combine(saveFolder, "BWChaosDiscordBot.exe");

            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
            using (Stream stream = Assembly.GetManifestResourceStream("BWChaos.Resources.BWChaosDiscordBot.exe"))
            {
                byte[] data;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }
                File.WriteAllBytes(exePath, data);
            }

            #endregion

            #region Set up server

            botProcess = new Process();
            botProcess.StartInfo.FileName = exePath;
            botProcess.StartInfo.WorkingDirectory = saveFolder;
            botProcess.StartInfo.UseShellExecute = true;
            botProcess.StartInfo.CreateNoWindow = true;
            botProcess.Start();

            GlobalVariables.WatsonClient = new WatsonWsClient("127.0.0.1", 8827, false);
            GlobalVariables.WatsonClient.ServerConnected += ClientConnectedToServer;
            GlobalVariables.WatsonClient.ServerDisconnected += ClientDisconnectedFromServer;
            GlobalVariables.WatsonClient.MessageReceived += ClientReceiveMessage;
            GlobalVariables.WatsonClient.Start();

            #endregion
        }

        private void PopulateEffects()
        {
            if (EffectHandler.AllEffects.Count == 0)
            {
                // Get all effects from the assembly
                asmEffects = (from t in Assembly.GetTypes()
                              where t.BaseType == typeof(EffectBase) && t != typeof(Template)
                              select (EffectBase)Activator.CreateInstance(t)).ToList();
            }
            else
            {
                EffectHandler.AllEffects.Clear();
                GetMelonPreferences();
            }

            // Actually populate the effects list
            foreach (var e in FilterEffects(asmEffects))
            {
                EffectHandler.AllEffects.Add(e.Name, e);
            }

            #region Local functions because fuck you

            IEnumerable<EffectBase> FilterEffects(List<EffectBase> effects)
            {
                return from e in effects
                       where e.Types == EffectTypes.NONE || // is this optimization?
                       IsEffectViable(e.Types)
                       select e;
            }

            bool IsEffectViable(EffectTypes eTypes)
            {
                foreach (var tuple in eTypesToPrefs)
                    if (eTypes.HasFlag(tuple.Item1) && !tuple.Item2) return false; //todid: this fucking works?????

                return true;
            }

            #endregion
        }

        private void LiveUpdateEffects()
        {
            // I'm not sure what this would do, but it probably doesn't hurt...
            EffectHandler.Instance.gameObject.SetActive(false);
            PopulateEffects();
            EffectHandler.Instance.gameObject.SetActive(true);
        }

        #endregion
    }

    internal class ChaosModStartupException : Exception
    {
        public ChaosModStartupException() : base("Illegal environment path", new Exception("Failed validating local path, try installing BONEWORKS on C:")) { }
    }
}