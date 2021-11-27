﻿using BWChaos.Effects;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
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

    public partial class Chaos : MelonMod
    {
        internal static bool isSteamVer = !File.Exists(Path.Combine(Application.dataPath, "..", "Boneworks_Oculus_Windows64.exe"));
        readonly static new Assembly Assembly = Assembly.GetExecutingAssembly(); // MelonMod's Assembly field isnt static so here we are
        internal static List<EffectBase> asmEffects = new List<EffectBase>();
        internal static List<(EffectTypes, bool)> eTypesToPrefs = new List<(EffectTypes, bool)>();
        public static Action<EffectBase> OnEffectRan;

        internal Process botProcess;

        public override void OnApplicationStart()
        {
            GlobalVariables.thisChaos = this; // so that we can access some instanced fields, like harmonylib for easy patching & unpatching

            #region Check datapath

            // Mathf.Sqrt(fish);
            if (isSteamVer && !(Path.GetFullPath(Path.Combine(Application.dataPath, "..")).EndsWith(@"BONEWORKS\BONEWORKS") || Application.dataPath.Contains("steamapps")))
                throw new ChaosModStartupException();

            #endregion

            #region MelonPref Setup

            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");
            // If MP's are gotten before they're registered in ML, an error is thrown.
            Prefs.Init();
            Prefs.Get();

            #endregion

            #region Load Timer

            // Load the Chaos UI elements. Don't change scope in case it may screw something up. idk why it would, but we're dontunloadunusedasset'ing it.
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
            // Load the AssetBundle straight from memory to avoid copying unnecessary files to disk
            Assembly.UseEmbeddedResource("BWChaos.Resources.effectresources", bytes => GlobalVariables.EffectResources = AssetBundle.LoadFromMemory(bytes));
            GlobalVariables.EffectResources.hideFlags = HideFlags.DontUnloadUnusedAsset; // IL2 BETTER NOT FUCK WITH MY SHIT

            MelonLogger.Msg("Loaded effect resources; All resource paths:");
            // Unity doesn't like executing the same method on an assetbundle more than once, so I need to cache the paths here in my own readonly list, because for
            // whatever reason, other IEnumerables seemed to get nulled in IL2's shitfuck domain. s/o to oBjEcT wAs GaRbAgE cOlLeCtEd In ThE iL2CpP dOmAiN
            GlobalVariables.ResourcePaths = GlobalVariables.EffectResources.GetAllAssetNames().ToList().AsReadOnly(); // use linq to cast lol
#if DEBUG
            foreach (var path in GlobalVariables.ResourcePaths)
                MelonLogger.Msg(path);
#endif

            MelonLogger.Msg("Loading him");
            Assembly.UseEmbeddedResource("BWChaos.Resources.jevil", bytes => ModThatIsNotMod.CustomItems.LoadItemsFromBundle(AssetBundle.LoadFromMemory(bytes)));

            MelonLogger.Msg("Done loading effect resources");

            #endregion

            #region Initialize from MelonPrefs & init effects

            PopulateEffects();
            if (Prefs.SyncEffects) Extras.EntanglementSyncHandler.Init();
            if (Prefs.EnableRemoteVoting)
            {
                // Discord IDs are ulongs, twitch IDs are strings, so if it fails to parse, then its not a discord channel
                //Prefs.isTwitch = !ulong.TryParse(Prefs.channelId, out ulong _); commented cause nothing fucking uses it
                StartBot();
            }

            #endregion

#if DEBUG
            MelonLogger.Msg($"Of {asmEffects.Count} total effects, {EffectHandler.AllEffects.Count} are present.");
#endif

            BoneMenu.Register();
        }

        public override void OnApplicationQuit()
        {
            // If they were started, stop the clients and their processes.
            GlobalVariables.WatsonClient?.Stop();
            botProcess?.Kill();
            botProcess?.Dispose();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // you already know what the fuck goin on
            if (EffectHandler.AllEffects.Count < 1) while (true) { }

            // Grab the necessary references when the scene starts. 
            GlobalVariables.Player_BodyVitals =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.BodyVitals>();
            GlobalVariables.Player_RigManager =
                GameObject.FindObjectOfType<StressLevelZero.Rig.RigManager>();
            GlobalVariables.Player_Health =
                GameObject.FindObjectOfType<Player_Health>();
            GlobalVariables.Player_PhysBody =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.PhysBody>();
            GlobalVariables.MusicMixer =
                GameObject.FindObjectOfType<Data_Manager>().audioManager.audioMixer.FindMatchingGroups("Music").FirstOrDefault(m => m.name == "Music");
            // Separate cameras because it's better this way, I think. It's more distinguishable even if it requires two lines to keep the two "in sync"
            GlobalVariables.SpectatorCam =
                GameObject.Find("[RigManager (Default Brett)]/[SkeletonRig (GameWorld Brett)]/Head/FollowCamera").GetComponent<Camera>();
            GlobalVariables.Cameras =
                GameObject.FindObjectsOfType<Camera>().Where(c => c.name.StartsWith("Camera (")).ToArray();

            new GameObject("ChaosUIEffectHandler").AddComponent<EffectHandler>();
            EffectHandler.advanceTimer = sceneName != "scene_mainMenu" && sceneName != "scene_introStart";

            GameObject.FindObjectsOfType<StressLevelZero.Pool.Pool>().FirstOrDefault(p => p.name == "pool - Jevil").Prefab.GetComponent<AudioSource>().outputAudioMixerGroup =
                GlobalVariables.MusicMixer;
        }

        public override void OnUpdate()
        {
            foreach (EffectBase effect in GlobalVariables.ActiveEffects)
                effect.OnEffectUpdate();
        }

        // If MelonPreferences.cfg is saved while the game is open, make sure the changes are reflected in real time.
        public override void OnPreferencesLoaded() => LiveUpdateEffects();

#if DEBUG
        private readonly int horizStart = 5;
        private readonly int vertStart = 25;
        private readonly int width = 200;
        private readonly int height = 25;
        private readonly int gap = 5;
        // IMGUI for flatscreen debugging (for smoke testing new effects)
        public override void OnGUI()
        {
            if (Prefs.enableIMGUI)
            {
                var horizOffset = horizStart;
                // because otherwise, it clips into unityexplorers top bar lol
                var vertOffset = vertStart;
                for (int i = 0; i < EffectHandler.AllEffects.Count; i++)
                {
                    if (vertOffset + height + gap > Screen.height)
                    {
                        vertOffset = vertStart;
                        horizOffset += width + gap;
                    }
                    var e = EffectHandler.AllEffects.Values.ElementAt(i);
                    if (GUI.Button(new Rect(horizOffset, vertOffset, width, height), e.Name)) e.Run();
                    vertOffset += height + gap;
                }
            }
        }
#endif

        #region Websocket Methods

        private async void ClientConnectedToServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Connected to the bot!");
            await GlobalVariables.WatsonClient.SendAsync("ignorerepeatvotes:" + Prefs.IgnoreRepeatVotes);
            // Send data for startup then clear it out so that there's less of an opportunity for reflection to steal shit (i think)
            Prefs.SendBotInitalValues(); // doesnt really matter if we await this
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

        private void StartBot()
        {
            MelonLogger.Msg("Unpacking and starting remote voting process...");

            #region Extract Bot

            string saveFolder = Path.Combine(Path.GetTempPath(), "BW-Chaos");
            string exePath = Path.Combine(saveFolder, "BWChaosDiscordBot.exe");

            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
            if (File.Exists(exePath)) File.Delete(exePath);
            
            using (Stream stream = Assembly.GetManifestResourceStream("BWChaos.Resources.BWChaosDiscordBot.zip"))
            {
                var buffer = new byte[4096];

                using (var zipFile = new ZipFile(stream))
                using (var zipStream = zipFile.GetInputStream(zipFile[0]))
                using (Stream fsOut = File.Create(exePath))
                    StreamUtils.Copy(zipStream, fsOut, buffer);
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

        private static void PopulateEffects()
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
                Prefs.Get();
            }

            // Actually populate the effects list
            foreach (var e in FilterEffects(asmEffects))
            {
                EffectHandler.AllEffects.Add(e.Name, e);
            }


            foreach (var str in Prefs.ForceEnabledEffects)
            {
#if DEBUG
                MelonLogger.Msg("Force enabling effect '" + str + "' because it was in the melonprefs array");
#endif

                if (EffectHandler.AllEffects.Keys.Contains(str)) continue; // we dont want it in the list twice

                EffectBase effect = asmEffects.FirstOrDefault(e => e.Name == str); // firstordefault my beloved

                // If the effect name doesn't exist, "throw" an error
                if (effect == null)
                {
                    MelonLogger.Error($"Force enabled effect '{str}' wasn't found! Check MelonPreferences, are you sure that's the right name for the effect?");
                    continue;
                }

                // don't allow oculus players to try and crash my shit
                if (effect.Types.HasFlag(EffectTypes.USE_STEAM) && !isSteamVer)
                {
                    MelonLogger.Warning("This is the Oculus version, however you attempted to use a Steam effect! This is not allowed! Are you trying to crash your game???");
                    continue;
                }

                EffectHandler.AllEffects.Add(str, effect);


            }

            #region Local function because fuck you

            IEnumerable<EffectBase> FilterEffects(IEnumerable<EffectBase> effects)
            {
                return from e in effects
                       where e.Types == EffectTypes.NONE || // is this optimization?
                       (IsEffectViable(e.Types) &&
                       !Prefs.ForceDisabledEffects.Contains(e.Name))
                       select e;
            }

            #endregion
        }

        internal static bool IsEffectViable(EffectTypes eTypes)
        {
            foreach (var tuple in eTypesToPrefs)
                if (eTypes.HasFlag(tuple.Item1) && !tuple.Item2) return false; //todid: this fucking works?????

            return true;
        }

        internal static void LiveUpdateEffects()
        {
            // I'm not sure what this would do, but it probably doesn't hurt...
            EffectHandler.Instance.gameObject.SetActive(false);
            PopulateEffects();
            foreach (var e in GlobalVariables.ActiveEffects.Where(e => !IsEffectViable(e.Types))) e.ForceEnd();
            EffectHandler.Instance.gameObject.SetActive(true);
        }

        #endregion
    }

    internal class ChaosModStartupException : Exception
    {
        public ChaosModStartupException() : base($"Illegal environment path '{MelonUtils.GameDirectory}'", new Exception("Failed validating local path, try installing BONEWORKS on C:")) { }
    }

    internal class ChaosModRuntimeException : ChaosModStartupException
    {
        public ChaosModRuntimeException() : base() { }
    }
}