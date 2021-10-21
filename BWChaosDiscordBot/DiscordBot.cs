using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;
using static BWChaosRemoteVoting.GlobalVariables;

namespace BWChaosRemoteVoting
{
    static class DiscordBot
    {
        public static DiscordClient discordClient;
        public static DiscordChannel discordChannel;

        public static async Task Init(string token, ulong channelId)
        {
            try
            {
                discordClient = new DiscordClient(new DiscordConfiguration()
                {
                    Token = token,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged
                });
                discordClient.MessageCreated += DiscordMessageSent;
                await discordClient.ConnectAsync();
                discordChannel = await discordClient.GetChannelAsync(channelId);
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
            if (ignoreRepeats && users.Contains(e.Author.Id.ToString())) return null;
            users.Add(e.Author.Id.ToString());

            if (int.TryParse(e.Message.Content, out int messageInt))
            {
                messageInt -= num;
                if (messageInt >= 1 && messageInt <= 5)
                    accumulatedVotes[messageInt - 1]++;
            }

            return Task.CompletedTask;
        }
    }
}