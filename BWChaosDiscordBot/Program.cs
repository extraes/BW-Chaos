using System;
using System.Text;
using System.Threading.Tasks;

using WatsonWebsocket;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace BWChaosDiscordBot
{
    internal class Program
    {
        private static WatsonWsServer watsonServer;
        private static string currentClientIpPort;

        private static DiscordClient discordClient;
        private static DiscordChannel discordChannel;
        private static string discordBotToken;
        private static string discordChannelId;

        private static int[] accumulatedVotes = new int[5] { 0, 0, 0, 0, 0 };

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
            watsonServer.Start();

            Console.WriteLine("Created server!");

            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            // wait until we have token and channel id
            while (string.IsNullOrEmpty(discordChannelId)) await Task.Delay(250);

            try
            {
                discordClient = new DiscordClient(new DiscordConfiguration()
                {
                    Token = discordBotToken,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged
                });
                discordClient.MessageCreated += DiscordMessageSent;
                await discordClient.ConnectAsync();
                discordChannel = await discordClient.GetChannelAsync(ulong.Parse(discordChannelId));
                await watsonServer.SendAsync(currentClientIpPort, "log:Connected to Discord and fetched the channel.");
            }
            catch (Exception e)
            {
                await watsonServer.SendAsync(currentClientIpPort, "error:" + e.ToString());
                //Console.WriteLine(e.ToString());
            }

            await Task.Delay(-1);
        }

        private static Task DiscordMessageSent(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Channel != discordChannel) return null;

            if (int.TryParse(e.Message.Content, out int messageInt))
            {
                if (messageInt >= 1 && messageInt <= 5)
                    accumulatedVotes[messageInt - 1]++;
            }

            return Task.CompletedTask;
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
                discordClient.DisconnectAsync().Wait();
                discordClient.Dispose();
                watsonServer.Stop();
                watsonServer.Dispose();
                Environment.Exit(0);
            }
        }

        private static void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Console.WriteLine("Message received from " + e.IpPort + ": " + Encoding.UTF8.GetString(e.Data));
            string[] splitMessage = Encoding.UTF8.GetString(e.Data).Split(':', 2);
            string messageType = splitMessage[0];
            string messageData = splitMessage[1] ?? string.Empty;
            switch (messageType)
            {
                case "token":
                    discordBotToken = messageData;
                    break;
                case "channel":
                    discordChannelId = messageData;
                    break;
                case "sendvotes":
                    watsonServer.SendAsync(currentClientIpPort, JsonConvert.SerializeObject(accumulatedVotes, Formatting.None));
                    for (int i = 0; i < accumulatedVotes.Length; i++)
                        accumulatedVotes[i] = 0;
                    break;
                case "sendtochannel":
                    discordClient.SendMessageAsync(discordChannel, messageData);
                    break;
                default:
                    Console.WriteLine("UNKNOWN MESSAGE TYPE: " + messageType);
                    break;
            }
        }

        #endregion
    }
}
