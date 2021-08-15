using BWChaos.Effects;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

    public class BWChaos : MelonMod
    {
        internal string botToken = "YOUR_TOKEN_HERE";
        internal string channelId = "CHANNEL_ID_HERE";
        internal static bool isSteamVer = !File.Exists(Path.Combine(Application.dataPath, "..", "Boneworks_Oculus_Windows64.exe")); //todo: get an oculus player to check if this works
        internal bool randomOnNoVotes = false;
        internal static bool useLaggyEffects = false;
        internal static bool useGravityEffects = false;
        internal static bool useSteamProfileEffects = false;
        internal static List<(EffectTypes, bool)> eTypesToPrefs = new List<(EffectTypes, bool)> { (EffectTypes.USE_STEAM, isSteamVer) };

        internal Process botProcess;

        public override void OnApplicationStart()
        {
            #region MelonPref Setup

            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");

            MelonPreferences.CreateEntry("BW_Chaos", "token", botToken, "token");
            botToken = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");

            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelId, "channel");
            channelId = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");

            MelonPreferences.CreateEntry("BW_Chaos", "randomEffectOnNoVotes", randomOnNoVotes, "randomEffectOnNoVotes");
            randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");

            MelonPreferences.CreateEntry("BW_Chaos", "useGravityEffects", useGravityEffects, "useGravityEffects");
            useGravityEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useGravityEffects");
            eTypesToPrefs.Add((EffectTypes.AFFECT_GRAVITY, useGravityEffects));

            MelonPreferences.CreateEntry("BW_Chaos", "useSteamProfileEffects", useSteamProfileEffects, "useSteamProfileEffects");
            useSteamProfileEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects");
            eTypesToPrefs.Add((EffectTypes.AFFECT_STEAM_PROFILE, useSteamProfileEffects));

            MelonPreferences.CreateEntry("BW_Chaos", "useLaggyEffects", useLaggyEffects, "useLaggyEffects");
            useLaggyEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useLaggyEffects");
            eTypesToPrefs.Add((EffectTypes.LAGGY, useLaggyEffects));

            MelonPreferences.Save();

            #endregion

            #region Default Checks

            if (botToken == "YOUR_TOKEN_HERE" || channelId == "CHANNEL_ID_HERE")
            {
                MelonLogger.Warning("BW Chaos will remain inactive util the BOT token and channel id are set inside modprefs.");
                MelonLogger.Warning("User tokens are against TOS and thus unsupported.");
                return;
            }

            #endregion

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

            EffectHandler.AllEffects = (from t in Assembly.GetTypes()
                where t.BaseType == typeof(EffectBase) && t != typeof(Template)
                select (EffectBase)Activator.CreateInstance(t)).ToList();

            #region Remove effects based on flags
            // todo: do this better, cause this is fucking stupid - extraes, the dumbfuck writer of this shitty ass block of linq
            EffectHandler.AllEffects = (from e in EffectHandler.AllEffects
                where e.Types.HasFlag(EffectTypes.NONE) || // is this optimization?
                      (e.Types.HasFlag(EffectTypes.USE_STEAM) && isSteamVer) &&
                      (e.Types.HasFlag(EffectTypes.AFFECT_GRAVITY) && useGravityEffects) &&
                      (e.Types.HasFlag(EffectTypes.AFFECT_STEAM_PROFILE) && useSteamProfileEffects) &&
                      (e.Types.HasFlag(EffectTypes.LAGGY) && useLaggyEffects)
                select e).ToList();
            #endregion

            #region BoneMenu for Debugging

            MenuCategory menu = MenuManager.CreateCategory("Chaos", Color.white);
            foreach (EffectBase effect in EffectHandler.AllEffects)
                menu.CreateFunctionElement(effect.Name, Color.white, () => effect.Run());

            #endregion

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

            bool IsEffectViable(EffectTypes eTypes)
            {
                foreach (var tuple in eTypesToPrefs) 
                    if (!(eTypes.HasFlag(tuple.Item1) && tuple.Item2)) return false; //todo: does this fucking work?????????????
                
                return true;
            }
        }

        public override void OnApplicationQuit()
        {
            GlobalVariables.WatsonClient?.Stop();
            botProcess?.Kill();
            botProcess?.Dispose();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GlobalVariables.Player_BodyVitals =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.BodyVitals>();
            GlobalVariables.Player_RigManager =
                GameObject.FindObjectOfType<StressLevelZero.Rig.RigManager>();
            GlobalVariables.Player_Health =
                GameObject.FindObjectOfType<Player_Health>();
            GlobalVariables.Player_PhysBody =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.PhysBody>();

            new GameObject("ChaosUIEffectHandler").AddComponent<EffectHandler>();
        }

        public override void OnUpdate()
        {
            foreach (EffectBase effect in GlobalVariables.ActiveEffects)
                effect.OnEffectUpdate();
        }

        #region Websocket Methods

        private async void ClientConnectedToServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Connected to the Discord bot!");
            await GlobalVariables.WatsonClient.SendAsync("token:" + botToken);
            await GlobalVariables.WatsonClient.SendAsync("channel:" + channelId);
        }

        private void ClientDisconnectedFromServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Disconnected from the Discord bot, was the process killed?");
        }

        private void ClientReceiveMessage(object sender, MessageReceivedEventArgs e)
        {
            //MelonLogger.Msg("Message received from " + e.IpPort + ": " + Encoding.UTF8.GetString(e.Data));
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
    }
}