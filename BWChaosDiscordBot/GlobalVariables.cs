using System;
using System.Collections.Generic;
using System.Text;
using WatsonWebsocket;

namespace BWChaosRemoteVoting
{
    public static class GlobalVariables
    {
        public static bool ignoreRepeats = false;
        public static int[] accumulatedVotes = new int[5] { 0, 0, 0, 0, 0 };
        public static WatsonWsServer watsonServer;
        public static string currentClientIpPort;
        public static List<string> users = new List<string>();
    }
}
