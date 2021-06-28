using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;
using MelonLoader;
using WatsonWebsocket;
using BW_Chaos.Effects;
using System.Threading.Tasks;

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

        private List<EffectBase> effectList = new List<EffectBase>();
        private List<EffectBase> candidateEffects = new List<EffectBase>();
        private float timeSinceEnabled = 0f;
        private int whenTimerReset = 0;
        private bool showGUI = false;

        public override void OnApplicationStart()
        {
            #region MelonPref Setup

            MelonPreferences.CreateCategory("BW_Chaos", "BW_Chaos");

            MelonPreferences.CreateEntry("BW_Chaos", "token", botToken, "token");
            botToken = MelonPreferences.GetEntryValue<string>("BW_Chaos", "token");

            MelonPreferences.CreateEntry("BW_Chaos", "channel", channelId, "channel");
            channelId = MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel");

            // todo: dviperHmm
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

            #region Extract Bot

            string saveFolder = Path.Combine(Path.GetTempPath(), "BW-Chaos");
            string exePath = Path.Combine(saveFolder, "BWChaosDiscordBot.exe");

            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
            using (Stream stream = Assembly.GetManifestResourceStream("BW_Chaos.BWChaosDiscordBot.exe"))
            {
                byte[] data;
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    data = ms.ToArray();
                }
                if (File.Exists(exePath)) File.Delete(exePath);
                File.WriteAllBytes(exePath, data);
            }

            #endregion

            // todo: test this
            effectList = (from t in Assembly.GetTypes()
             where t.GetInterfaces().Contains(typeof(EffectBase)) && t.GetConstructor(Type.EmptyTypes) != null
             select (EffectBase)Activator.CreateInstance(t)).ToList();

            botProcess = new Process();
            botProcess.StartInfo.FileName = exePath;
            botProcess.StartInfo.WorkingDirectory = saveFolder;
            botProcess.StartInfo.UseShellExecute = true;
            botProcess.StartInfo.CreateNoWindow = false; // todo: check if this actually works
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
            GlobalVariables.Player_RigManager =
                GameObject.FindObjectOfType<StressLevelZero.Rig.RigManager>();
            GlobalVariables.Player_Health =
                GameObject.FindObjectOfType<Player_Health>();
            GlobalVariables.Player_PhysBody =
                GameObject.FindObjectOfType<StressLevelZero.VRMK.PhysBody>();
        }

        public override void OnUpdate()
        {
            foreach (EffectBase effect in GlobalVariables.ActiveEffects)
                effect.OnEffectUpdate();
        }

        public override void OnGUI()
        {
            if (showGUI)
            {
                float timeSinceReset = Time.timeSinceLevelLoad - timeSinceEnabled;
                if (!(timeSinceReset >= 30))
                {
                    GUI.Box(new Rect(50, 25, 350, 25),
                        "BW Chaos: Waiting " + (30 - System.Math.Floor(timeSinceReset)) + " seconds before starting");
                    GUI.Box(new Rect(50, 50, 350, 25), "Made by extraes");
                    return;
                }
                int effectNumber = 0;
                foreach (EffectBase effect in candidateEffects)
                {
                    GUI.Box(new Rect(50, 50 + (effectNumber * 25), 500, 25), $"{effectNumber + 1}: {effect.Name}");
                    effectNumber++;
                }
                GUI.Box(new Rect(50, 50 + (effectNumber * 25), 500, 25), $"{effectNumber + 1}: Random Effect");
                GUI.Box(new Rect(50, 250, 500, (GlobalVariables.ActiveEffects.Count + 1) * 20 + 10), "Active effects:\n" + string.Join("\n", GlobalVariables.ActiveEffects));
                GUI.Box(new Rect(Screen.width - 550, 50, 500, 25), "Time");
                GUI.Box(new Rect(Screen.width - 550, 75, 500 * System.Math.Min(timeSinceReset % 30 / 30, 1f), 25), "");

                if ((timeSinceReset / 30) >= whenTimerReset)
                {
                    whenTimerReset++;
                    RunVotedEffect();
                    ResetEffectCandidates();
                }
            }
        }

        private async void RunVotedEffect()
        {
            await watsonClient.SendAsync("sendvotes:");
            while (accumulatedVotes == null) await Task.Delay(250);
            (int, int) topVoted = (0, 0); // format is (arrIndex, value)
            for (int i = 0; i < accumulatedVotes.Length; i++)
            {
                if (accumulatedVotes[i] > topVoted.Item2)
                    topVoted = (i, accumulatedVotes[i]);
            }
            if (topVoted.Item1 == 4) effectList[UnityEngine.Random.Range(0, effectList.Count)].Run();
            else candidateEffects[topVoted.Item1].Run();
        }

        private void ResetEffectCandidates()
        {
            candidateEffects.Clear();
            for (int i = 0; i < 4; i++)
            {
                EffectBase effect = effectList[UnityEngine.Random.Range(0, effectList.Count)];
                while (candidateEffects.Contains(effect))
                    effect = effectList[UnityEngine.Random.Range(0, effectList.Count)];
                candidateEffects.Add(effect);
            }
        }

        #region Websocket Methods

        private int[] accumulatedVotes = null;

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
                    accumulatedVotes = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(messageData);
                    break;
                default:
                    MelonLogger.Error("UNKNOWN MESSAGE TYPE: " + messageType);
                    break;
            }
        }

        #endregion
    }
}