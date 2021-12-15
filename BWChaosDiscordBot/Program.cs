using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebsocket;
using TwitchLib;
using TwitchLib.Api;
using static BWChaosRemoteVoting.GlobalVariables;

namespace BWChaosRemoteVoting
{
    internal class Program
    {   
        private static string botToken;
        private static string channelIdOrName;

        private static void Main()
        {
            /*
             * I have no clue if you will ever see this but to
             * compile this into a single EXE you need to do it a bit differently.
             * 1. Right click the project in Solution Explorer and select "Publish"
             * 2. There should already be a profile for a single-exe local publish,
             *  so just press the big Publish button and wait a few and it'll compile
             *  to `BWChaosDiscordBot\bin\Release\netcoreapp3.1\publish\`
            */

            watsonServer = new WatsonWsServer("127.0.0.1", 8827, false);
            watsonServer.ClientConnected += ClientConnected;
            watsonServer.ClientDisconnected += ClientDisconnected;
            watsonServer.MessageReceived += MessageReceived;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            watsonServer.Start();

            Console.WriteLine("Created server!");

            MainAsync().GetAwaiter().GetResult();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            watsonServer.SendAsync(currentClientIpPort, "error:" + (e.ExceptionObject as Exception).ToString());
        }

        private static async Task MainAsync()
        {
            // wait until we have token and channel id
            while (string.IsNullOrEmpty(channelIdOrName)) await Task.Delay(250);

            // Discord 
            if (ulong.TryParse(channelIdOrName, out ulong idUlong)) await DiscordBot.Init(botToken, idUlong);
            else await TwitchBot.Init(botToken, channelIdOrName);

            await Task.Delay(-1);
        }


        #region Websocket Methods

        private static void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            //Console.WriteLine("Client connected: " + args.IpPort);
            currentClientIpPort = args.IpPort;
        }

        private static void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            //Console.WriteLine("Client disconnected: " + args.IpPort);
            if (currentClientIpPort == args.IpPort)
            {
                watsonServer.Stop();
                watsonServer.Dispose();
                TwitchBot.twitchClient?.Disconnect();
                DiscordBot.discordClient?.DisconnectAsync()?.Wait();
                DiscordBot.discordClient?.Dispose();
                Environment.Exit(0);
            }
        }

        private static void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string[] splitMessage = Encoding.UTF8.GetString(e.Data).Split(':');
            string messageType = splitMessage[0];
            string messageData = string.Join(":", splitMessage?.Skip(1)?.ToArray()) ?? string.Empty;
            switch (messageType)
            {
                case "token":
                    botToken = messageData;
                    break;
                case "channel":
                    channelIdOrName = messageData;
                    break;
                case "sendvotes":
                    watsonServer.SendAsync(currentClientIpPort, JsonConvert.SerializeObject(accumulatedVotes, Formatting.None));
                    break;
                case "clearvotes":
                    for (int i = 0; i < accumulatedVotes.Length; i++)
                        accumulatedVotes[i] = 0;
                    users.Clear();
                    break;
                case "ignorerepeatvotes":
                    ignoreRepeats = bool.Parse(messageData);
                    break;
                case "sendtochannel":
                    DiscordBot.discordClient?.SendMessageAsync(DiscordBot.discordChannel, messageData);
                    break;
                case "flipnumbers":
                    num = int.Parse(messageData);
                    break;
                default:
                    Console.WriteLine("UNKNOWN MESSAGE TYPE: " + messageType);
                    break;
            }
        }

        #endregion
    }
}
