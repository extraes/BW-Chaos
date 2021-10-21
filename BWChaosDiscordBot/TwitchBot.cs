using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.V5;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using WatsonWebsocket;
using static BWChaosRemoteVoting.GlobalVariables;

namespace BWChaosRemoteVoting
{
    static class TwitchBot
    {
        public static TwitchClient twitchClient;

        public static async Task Init(string token, string channelName)
        {
            try
            {
                twitchClient = new TwitchClient();
                twitchClient.Initialize(new ConnectionCredentials(channelName, token), channelName);
                twitchClient.Connect();
                twitchClient.JoinChannel(channelName);

                twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
                for (int i=0; i<9; i++) 
                    twitchClient.AddChatCommandIdentifier((char)(i + 49));


                await watsonServer.SendAsync(currentClientIpPort, "log:Connected to Twitch.");
            }
            catch (Exception e)
            {
                await watsonServer.SendAsync(currentClientIpPort, "error:" + e.ToString());
            }

            await Task.Delay(-1);
        }

        private static void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            if (ignoreRepeats && users.Contains(e.Command.ChatMessage.UserId)) return;
            users.Add(e.Command.ChatMessage.UserId);

            if (int.TryParse(e.Command.CommandIdentifier.ToString(), out int messageInt))
            {
                // fuck this is bad code
                if (e.Command.ChatMessage.ToString().StartsWith("10")) messageInt = 10;

                // decrement number by the numberflip amount to align it with the vote bounds
                messageInt -= num;
                if (messageInt >= 1 && messageInt <= 5)
                    accumulatedVotes[messageInt - 1]++;
            }
        }

        
    }
}
