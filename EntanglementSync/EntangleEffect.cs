using Discord;
using Entanglement.Network;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWChaos.Sync
{
    internal class EntangleEffect : Effects.EffectBase
    {
        public EntangleEffect() : base("Get Fucking Entangled", 5) { }

        private static readonly string[] smallTalk = new string[] { 
            "how's the weather?",
            "how're the kids?",
            "whats poppin",
            $"get doxxed {(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}",
        };
        private List<User> users = new List<User>(32);
        public override void OnEffectStart()
        {
            if (Node.activeNode == null || Node.activeNode.connectedUsers.Count == 0)
            {
                Utilities.SpawnAd("Whoops this effect isn't supposed to run if you've got no fr- I mean, you're not connected to anyone.");
                ForceEnd();
                return;
            }

            foreach (var user in Node.activeNode.connectedUsers)
                DiscordIntegration.userManager.GetUser(user, (Result res, ref User u) => { if (res == Result.Ok) users.Add(u); });
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            while (users.Count < Node.activeNode.connectedUsers.Count) yield return null; // cant do WaitUntil cause IL2 strips more than twitch thots when they get a prime sub
            foreach (var user in users)
            {
                yield return new UnityEngine.WaitForSecondsRealtime(1f);
                Utilities.SpawnAd("yo tell " + user.Username + " " + smallTalk[UnityEngine.Random.RandomRange(0,smallTalk.Length)] + " for me");
            }
        }
    }
    
}
