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
        public EntangleEffect() : base("Get Fucking Entangled") { }

        private static readonly string[] smallTalk = new string[] { 
            "how's the weather?",
            "how're the kids?",
            "whats poppin",
            $"get doxxed {(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}.{(int)UnityEngine.Random.value * 256}",
        };
        private List<User> users = new List<User>(32);
        private bool hasAllUsers = false;
        public override void OnEffectStart()
        {
            if (Node.activeNode == null || Node.activeNode.connectedUsers.Count == 0)
            {
                Utilities.SpawnAd("Whoops this effect isn't supposed to run if you've got no fr- I mean, you're not connected to anyone.");
                ForceEnd();
                return;
            }
            hasAllUsers = false;
            List<string> theHomies = new List<string>();


            foreach (var user in Node.activeNode.connectedUsers)
                DiscordIntegration.userManager.GetUser(user, (Result res, ref User u) => { if (res == Result.Ok) users.Add(u); });
            hasAllUsers = true;
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            while (!hasAllUsers) yield return null;
            foreach (var user in users)
            {
                Utilities.SpawnAd("yo tell " + user.Username + " " + smallTalk[UnityEngine.Random.RandomRange(0,smallTalk.Length)] + " for me");
            }
        }
    }
    
}
