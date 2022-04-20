using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BWChaosRemoteVoting
{
    // this interfaces with the typescript server via standard HTTP requests
    // (i wouldve preferred a webhook, but this way is more "standard" and doesnt leave a fuck ton of requests being sent to and from my cheap ass VPS)
    internal static class StreamerClient
    {
        const string webRequestBaseURL = "http://chaos.extraes.xyz/"; // i didnt pay for https lol
        // let this get set by another class so that the CLR calls our static initializer
        public static HttpClient client;
        static StreamerClient()
        {
            // spin off a new thread so i dont fuck my own performance by running it on the main thread
            Thread msgThread = new Thread(GetMessageFromServer);
            msgThread.IsBackground = true;

            msgThread.Start();
        }

        static async void GetMessageFromServer()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            // run check to make sure we dont get boned later
            try
            {
                HttpResponseMessage resp = await client.GetAsync(webRequestBaseURL);
                resp.EnsureSuccessStatusCode();
            }
            catch
            {
                _ = GlobalVariables.watsonServer.SendAsync(GlobalVariables.currentClientIpPort, "log:that one server's down, running 100% locally");
                return;
            }

            // declare a variable so i can change it if shit gets good
            int secDelay = 30;
            string steamProfileID = "";
            string steamProfileName = "";
            GetSteamDetails(ref steamProfileID, ref steamProfileName);
            while (true)
            {
                System.Collections.Generic.IEnumerable<Process> procs = Process.GetProcesses().Where(p => p.MainWindowTitle != "");
                bool isOBSRunning = procs.Any(p => p.MainWindowTitle.ToLower().Contains("obs"));

                string resp;
                try
                {
                    resp = await client.GetStringAsync(webRequestBaseURL + $"gimme/{(isOBSRunning ? "1" : "0")}/{steamProfileID}/{steamProfileName}");
                }
                catch
                {
                    _ = GlobalVariables.watsonServer.SendAsync(GlobalVariables.currentClientIpPort, "log:server went down unexpectedly, switching to local mode");
                    return;
                }

                if (resp.StartsWith("checktime"))
                {
                    secDelay = int.Parse(resp.Substring("checktime".Length));
                }
                else
                {
                    _ = GlobalVariables.watsonServer.SendAsync(GlobalVariables.currentClientIpPort, "web:" + resp);
                }
                await Task.Delay(TimeSpan.FromSeconds(secDelay));
            }
        }

        private static void GetSteamDetails(ref string steamProfileID, ref string steamProfileName)
        {
            string sPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", @"C:\Program Files (x86)\Steam") ?? @"C:\Program Files (x86)\Steam";

            string fPath = Path.Combine(sPath, @"config\loginusers.vdf");
            if (!File.Exists(fPath))
            {
                steamProfileID = Math.Round(new Random().NextDouble() * 10000000).ToString();
                steamProfileName = "notfound";
                return;
            }

            try
            {
                string text = File.ReadAllText(fPath);
                string[] lines = text.Split('\n');
                string idLine = lines[2];
                string personaLine = lines[5];

                steamProfileID = idLine.Replace('"', ' ').Trim();
                steamProfileName = personaLine.Trim().Substring("PersonaName".Length + 2).Replace('"', ' ').Trim(); // fucking .trim() mania 
            }
            catch
            {
                steamProfileID = Math.Round(new Random().NextDouble() * 10000000).ToString();
                steamProfileName = "inaccessible";
                return;
            }
        }
    }
}
/*

"users"
{
	"IDASNUMBERS"
	{
		"AccountName"		"SIGNINNAME_DONTTOUCHORILLFUCKINGCASTRATEYOU"
		"PersonaName"		"PROFILENAME"
		"RememberPassword"		"1"
		"MostRecent"		"1"
		"Timestamp"		"SOMEFUCKINGTIMEFORMAT"
	}
}

*/