using System;
using System.Text;
using System.Diagnostics;

using UnityEngine;
using MelonLoader;
using WatsonWebsocket;

namespace BW_Chaos
{
    public static class BuildInfo
    {
        public const string Name = "BW_Chaos";
        public const string Author = "extraes";
        public const string Company = null;
        public const string Version = "0.1.2";
        public const string DownloadLink = null;      
    }

    public class BW_Chaos : MelonMod
    {
        internal string botToken = "YOUR_TOKEN_HERE";
        internal string channelId = "CHANNEL_ID_HERE";

        internal WatsonWsClient watsonClient;
        internal Process botProcess;

        public override void OnApplicationStart()
        {
            #region MelonPref Setup

            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");

            MelonPreferences.CreateEntry("BW_Chaos", "token", botToken, "token");
            botToken = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");

            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelId, "channel");
            channelId = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");

            // todo: :dviperHmm:
            MelonPreferences.Save(); // BECAUSE IF I DONT SAVE IT RN THEN THE FAT BASTARD WONT CHANGE IT UNTIL THE GAME CLOSES

            #endregion

            #region Default Checks

            if (botToken == "YOUR_TOKEN_HERE" || channelId == "CHANNEL_ID_HERE")
            {
                MelonLogger.Warning("BW Chaos will remain inactive util the BOT token and channel id are set inside modprefs.");
                MelonLogger.Warning("User tokens are against TOS and thus unsupported.");
                return;
            }

            #endregion

            botProcess = new Process();
            botProcess.StartInfo.FileName =
                @"C:\Users\trevo\Documents\GitHub\BW-Chaos-new\BWChaosDiscordBot\bin\Debug\netcoreapp3.1\BWChaosDiscordBot.exe";
            botProcess.StartInfo.WorkingDirectory =
                @"C:\Users\trevo\Documents\GitHub\BW-Chaos-new\BWChaosDiscordBot\bin\Debug\netcoreapp3.1\";
            botProcess.StartInfo.UseShellExecute = true;
            // botProcess.StartInfo.CreateNoWindow = false;
            botProcess.Start();

            watsonClient = new WatsonWsClient("127.0.0.1", 8827, false);
            watsonClient.ServerConnected += ClientConnectedToServer;
            watsonClient.ServerDisconnected += ClientDisconnectedFromServer;
            watsonClient.MessageReceived += ClientReceiveMessage;
            watsonClient.Start();
        }

        public override void OnApplicationQuit()
        {
            watsonClient?.Stop();
            botProcess?.Kill();
            botProcess?.Dispose();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GlobalVariables.Player_BodyVitals =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.BodyVitals>();
        }

        #region Websocket Methods

        private async void ClientConnectedToServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Connected to the Discord bot!");
            await watsonClient.SendAsync("token:" + botToken);
            await watsonClient.SendAsync("channel:" + channelId);
        }

        private void ClientDisconnectedFromServer(object sender, EventArgs e)
        {
            MelonLogger.Msg("Disconnected from the Discord bot, was the process killed?");
        }

        private void ClientReceiveMessage(object sender, MessageReceivedEventArgs e)
        {
            //MelonLogger.Msg("Message received from " + e.IpPort + ": " + Encoding.UTF8.GetString(e.Data));
            string messageType = Encoding.UTF8.GetString(e.Data).Split(':')[0];
            string messageData = Encoding.UTF8.GetString(e.Data).Split(':')[1] ?? string.Empty;
            switch (messageType)
            {
                case "error":
                    MelonLogger.Error("An error has occured within the Discord bot!");
                    MelonLogger.Error(messageData);
                    break;
                case "log":
                    MelonLogger.Msg(messageData);
                    break;
                case "receivevotes":
                    // todo:
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