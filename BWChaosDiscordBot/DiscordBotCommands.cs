using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using static BWChaosRemoteVoting.GlobalVariables;

namespace BWChaosRemoteVoting
{
    class DiscordBotCommands : ApplicationCommandModule
    {
        [SlashCommand("vote", "Vote for a BWChaos effect")]
        public async Task Vote(InteractionContext ctx,
            [Option("effect", "The number of the effect in the list")]
            long number)
        {
            bool reject = false;
            string content = "Acknowledged your vote!";
            number -= GlobalVariables.num;

            if (ignoreRepeats) // hasnt had the opportunity to be rejected yet (but still prevent them from spamming rejectable votes)
            {
                if (users.Contains(ctx.Member.Id.ToString()))
                {
                    reject = true;
                    content = "You can't vote until the next voting cycle!";
                }
                else users.Add(ctx.Member.Id.ToString());
            }

            if (!reject && !(number >= 1 && number <= 5)) // if (not yet rejected) && (num is out of range)
            {
                reject = true;
                content = "You voted for an invalid effect number!";
            }

            if (!reject)
            {
                accumulatedVotes[number - 1]++;
                content = $"Acknowledged your vote for effect #{number - 1}!";
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            { 
                IsEphemeral = true,
                Content = content,
            });
        }
    }
}