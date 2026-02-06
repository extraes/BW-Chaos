using BLChaos.Effects;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.SceneStreaming;
using Jevil.IMGUI;
using Jevil.Patching;
using MelonLoader;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using MelonLoader.ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WatsonWebsocket;
using static BLChaos.Effects.EffectBase;

//[assembly:HarmonyDontPatchAll]

namespace BLChaos;


public static class BuildInfo
{
    public const string Name = "BLChaos";
    public const string Author = "extraes, trev";
    public const string Company = null;
    public const string Version = "1.0.0";
    public const string DownloadLink = "https://bonelab.thunderstore.io/package/BWChaosDevs/BL_Chaos/";
}

//todo: remove duplicate methods that exist in JeviLib 

// done: screen pixelation "Quest Port" (see https://discord.com/channels/563139253542846474/753783288031608923/1033455733758513362)
// done: no volumetrics (disable the global::VolumetricRendering component, or VolumetricRendering.disable/enable)
// done: make "stick drift" (see: https://discord.com/channels/563139253542846474/656631681406468137/1053036547810799686)
// done: make "no more chunks" effect (disables chunk loading)
// done: make "Simulation instability" effect (see: https://discord.com/channels/563139253542846474/716099004894806016/1100131543613186128)
// done: bloom "E3 2016" (see https://discord.com/channels/563139253542846474/753783288031608923/1033455814855372819)
// done: make metal head effect (see: https://discord.com/channels/@me/771537321744269333/1066097758810943558)

// todo: weezer effect "manipulator music" (see https://discord.com/channels/563139253542846474/656631681406468137/1032840669724676106)
// todo: change "My meme folder" -> change the material of monitors & spawn one (see: https://discord.com/channels/563139253542846474/753783288031608923/1037511912268771358)
// todo: make "Lego deconstruction" (see: https://discord.com/channels/563139253542846474/753783288031608923/1037513696060129302)
// todo: make "Bad to the bone" or "My movie" (whenever you spawn w/ the skeleton avatar or a skeleton NPC wakes up, play the bad to the bone riff)
// todo: make hotline miami effect (see: https://discord.com/channels/@me/771537321744269333/1069647422969622618)
// todo: https://discord.com/channels/563139253542846474/753783288031608923/1050226721577775154
// todo: make theatrigon effect (see: https://discord.com/channels/@me/771537321744269333/1069666211316645958)
// todo: make prop gun effect (make all gunshots spawn a random prop)
// todo: make "ran out of glue" effect (see: https://www.youtube.com/watch?v=W7P75jlHLHc)
// todo: make this effect, open process in window, capture window, show ingame (possibly as a screenspace postprocess thing?) https://discord.com/channels/563139253542846474/753774159645114410/1088278797700313129
// todo: make a G-Man speech effect. freeze time, lock player in place, g man speech + anim that draws over everything
// todo: make a Portal-1-Start effect, complete with "Still Alive - Radio Mix Clean"
// todo: Make a "Your jordans are fake" effect, sending an object flying when you point at it with your index finger
// todo: Make a "night enjoyers be like" effect that just blinds you by making you only see black.
// todo: make a low-health jesus image effect. reuse shader from sugarcoat
public class Chaos : MelonMod
{
    public Chaos() : base() => _instance = this;
    internal static bool isSteamVer = File.Exists(Path.Combine(Application.dataPath, "..", "Bonelab_Steam_Windows64.exe"));
    internal static bool isQuest = Utilities.IsPlatformQuest();
    internal static new readonly Assembly Assembly = Assembly.GetExecutingAssembly(); // MelonMod's Assembly field isnt static so here we are
    internal static Type[] AsmTypes = Assembly.GetTypes();
    private static Chaos _instance;
    public static Chaos Instance => _instance; // so that we can access some instanced fields, like harmonylib patching
    internal static List<EffectBase> asmEffects = new List<EffectBase>();
    internal static List<(EffectTypes, bool)> eTypesToPrefs = new List<(EffectTypes, bool)>();
    public static event Action<EffectBase>? OnEffectRan;

    private bool started = false;
    internal Process? botProcess;

    public override void OnInitializeMelon()
    {
#if NOBONELIB
        Hook.OntoMethod(typeof(JeviLib).GetMethod(nameof(JeviLib.OnSceneWasInitialized), BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? throw new MissingMethodException("JL OSWI"), InitializeReferences);
#endif

        //InitializeAsync().GetAwaiter().GetResult();
        AsyncUtilities.WrapNoThrow(InitializeAsync).RunOnFinish(LogStartupExceptionIfExists);
    }


    void LogStartupExceptionIfExists(Exception? ex)
    {
        if (ex is null) return;
        Error("Exception whilst initializing Chaos: ", ex);
    }

    public async Task InitializeAsync()
    {
        Stopwatch allSW = Stopwatch.StartNew();

#if DEBUG
        // ToArray to copy it to a separate collection
        DebugDraw.Button("Stop all effects", GUIPosition.TOP_RIGHT, () => GlobalVariables.ActiveEffects.ToArray().ForEach(e => e.ForceEnd()));
        using var dlc = new DebugLineCounter(LoggerInstance, DebugLineCounter.Kind.LINE_NUMBER, "Initializing");
        dlc.UpdateProgress();
#endif

        _ = AsyncUtilities.WrapNoThrow(SetupLatePatching).RunOnFinish(LogStartupExceptionIfExists);
        
        #region Check datapath

        // Mathf.Sqrt(fish);
        //if (isSteamVer && !(Path.GetFullPath(Path.Combine(Application.dataPath, "..")).EndsWith(@"BONEWORKS\BONEWORKS") || Application.dataPath.Contains("steamapps")))
        //    throw new ChaosModStartupException();

        #endregion

        #region MelonPref Setup

#if DEBUG
        dlc.UpdateProgress();
#endif
        // If MP's are gotten before they're registered in ML, an error is thrown.
        Chaos.Log("Creating preference entries...");
        Prefs.Init();
        Chaos.Log("Created preference entries, now retrieving values...");
#if DEBUG
        dlc.UpdateProgress();
#endif
        Prefs.Get();
        Chaos.Log("Successfully initialized preferences.");

        #endregion

        #region Load Timer

#if DEBUG
        dlc.UpdateProgress();
#endif
        // Load the Chaos UI elements. Don't change scope in case it may screw something up. idk why it would, but we're dontunloadunusedasset'ing it.
        string uiName = isQuest ? "questuielements" : "uielements";

        AssetBundle? uiBundle = null;
        byte[] uiBytes = null!;
#if DEBUG
        int attempt = 1;
#endif
        Assembly.UseEmbeddedResource($"BLChaos.Resources.{uiName}", bytes => uiBytes = bytes);

#if DEBUG
        dlc.UpdateProgress();
#endif
        while (uiBundle == null)
        {
            try
            {
#if DEBUG
                Chaos.Log("Attempting to load the UI assetbundle: attempt " + attempt);
                attempt++;
#endif
                uiBundle = await AssetBundle.LoadFromMemoryAsync(uiBytes).ToTask();
                if (uiBundle == null) throw new NullReferenceException("Failed to load UI assetbundle");
            }
            catch(VerificationException) // means INotifyCompletion is not implementing awaiters
            {
                throw;
            }
            catch
            {
                if (uiBundle == null)
                {
                    AssetBundle? bundle = AssetBundle.GetAllLoadedAssetBundles_Native().FirstOrDefault(b => b.AllAssetNames().Contains("Assets/UIStuff/prefabs/ChaosCanvas.prefab".ToLower()));
                    if (bundle != null)
                    {
                        Chaos.Log("Found the assetbundle in the loaded bundles, despite the load task being cancelled. Whatever. We ball.");
                        uiBundle = bundle;
                        break;
                    }
                }
            }
        }

#if DEBUG
        dlc.UpdateProgress();
        Chaos.Log("Loaded essentials assetbundle. All asset paths are below:");
        uiBundle.GetAllAssetNames().ForEach(str => Chaos.Log(" - " + str));
#endif

        GlobalVariables.WristChaosUI = uiBundle.LoadAsset("Assets/UIStuff/prefabs/ChaosCanvas.prefab").Cast<GameObject>();
        GlobalVariables.WristChaosUI.hideFlags = HideFlags.DontUnloadUnusedAsset;

        GlobalVariables.OverlayChaosUI = uiBundle.LoadAsset("Assets/UIStuff/prefabs/ChaosCanvasOverlay.prefab").Cast<GameObject>();
        GlobalVariables.OverlayChaosUI.hideFlags = HideFlags.DontUnloadUnusedAsset;
        Chaos.Log("Successfully initialized essential assets.");

        #endregion

        #region Load effect resources

        Stopwatch resSW = Stopwatch.StartNew();
        Chaos.Log("Loading effect resources, please wait...");
        // Load the AssetBundle straight from memory to avoid copying unnecessary files to disk
        string resourceName = isQuest ? "questeffectresources" : "effectresources";
        byte[]? effRes = null;
        Assembly.UseEmbeddedResource($"BLChaos.Resources.{resourceName}", bytes => effRes = bytes);
        GlobalVariables.EffectResources = await AssetBundle.LoadFromMemoryAsync(effRes).ToTask();
        //var t = AssetBundle.LoadFromMemoryAsync(effRes).ToTask();
        //while (!t.IsCompleted)
        //    await UniTask.Yield();
        ////if (effRes is null)
        ////    throw new Exception("Failed to load embedded assetbundle for effect resources");
        ////GlobalVariables.EffectResources = AssetBundle.LoadFromMemory(effRes);
        ////GlobalVariables.EffectResources.hideFlags = HideFlags.DontUnloadUnusedAsset; // IL2 BETTER NOT FUCK WITH MY SHIT

        // Unity doesn't like executing the same method on an assetbundle more than once, so I need to cache the paths here in my own readonly list, because for
        // whatever reason, other IEnumerables seemed to get nulled in IL2's shitfuck domain. s/o to oBjEcT wAs GaRbAgE cOlLeCtEd In ThE iL2CpP dOmAiN
        GlobalVariables.ResourcePaths = GlobalVariables.EffectResources.GetAllAssetNames().ToList().AsReadOnly(); // use linq to cast lol
#if DEBUG
        Chaos.Log("Loaded effect resources; All resource paths:");
        foreach (string path in GlobalVariables.ResourcePaths)
            Chaos.Log(path);
#endif
        resSW.Stop();
        Chaos.Log("Done loading effect resources");

        #endregion

        #region Initialize from MelonPrefs & init effects

        Stopwatch effectSW = Stopwatch.StartNew();
        PopulateEffects();
        effectSW.Stop();

        Stopwatch syncSW = Stopwatch.StartNew();
        if (Prefs.syncEffects) Extras.FusionSyncHandler.Init();
        syncSW.Stop();

        Stopwatch botSW = Stopwatch.StartNew();
        if (Prefs.enableRemoteVoting)
        {
            // Discord IDs are ulongs, twitch IDs are strings, so if it fails to parse, then its not a discord channel
            //Prefs.isTwitch = !ulong.TryParse(Prefs.channelId, out ulong _); commented cause nothing fucking uses it, the process can do it find on its own
            StartBot();
        }
        botSW.Stop();

        #endregion

        #region Do misc startup things

        Stopwatch miscSW = Stopwatch.StartNew();

#if !NOBONELIB
        BoneMenu.Register();
#endif
#if DEBUG

        dlc.UpdateProgress();

        DebugDraw.TrackVariable("ActiveEffects", GUIPosition.BOTTOM_RIGHT, () => GlobalVariables.ActiveEffects.Count);

        // flatscreen debugging
        Task<TestResult> res = Task.FromResult(TestResult.INCONCLUSIVE);
        bool doTest = false;
        DebugDraw.Button("Toggle Chaos button mode", GUIPosition.TOP_RIGHT, () => doTest = !doTest);
        DebugDraw.TrackVariable("Chaos button mode", GUIPosition.TOP_RIGHT, () => doTest ? "Testing" : "Run");
        DebugDraw.TrackVariable("Test status", GUIPosition.TOP_RIGHT, () => res.IsCompleted ? res.Result.ToString() : "Test incomplete");
        foreach (EffectBase eb in asmEffects.OrderBy(e => e.Name).ToArray())
        {
            GUIPosition pos = eb.Types == EffectTypes.NONE ? GUIPosition.TOP_LEFT : GUIPosition.BOTTOM_LEFT;
            DebugDraw.Button(eb.Name, pos, () =>
            {
                if (doTest)
                    res = eb.Test();
                else
                    eb.Run();
            });
        }

        dlc.UpdateProgress();

#endif
        started = true;

        miscSW.Stop();
        if (EffectHandler.allEffects.TryGetValue(Prefs.effectOnSceneLoad, out EffectBase effect))
            Stats.EffectCalledManuallyCallback(effect);
        // basically just allow http connections. why? uhhhh.... testing necessitated it? i dont think it breaks anything so uhhhh cool ig
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

#if NOBONELIB
        SceneStreamer.doAnyLevelLoad += new Action(() => InitializeReferences(-1, SceneStreamer.Session._level.Barcode.ID));
#else
        Hooking.OnLevelLoaded += li => InitializeReferences(-1, li.barcode);
#endif

        #endregion

        #region Output startup times

        allSW.Stop();
        // go straight to loggerinstance because it lets me use pretty colors :^)
        LoggerInstance.Msg(System.ConsoleColor.Blue, $"Started successfully in {allSW.ElapsedMilliseconds}ms: {asmEffects.Count} total effects, with {EffectHandler.allEffects.Count} to be used in Chaos.");
        LoggerInstance.Msg(System.ConsoleColor.Blue, $" - Effect initialization: {effectSW.ElapsedMilliseconds}ms");
        LoggerInstance.Msg(System.ConsoleColor.Blue, $" - Effect resource loading: {resSW.ElapsedMilliseconds}ms");
        LoggerInstance.Msg(System.ConsoleColor.Blue, $" - Misc startup tasks: {miscSW.ElapsedMilliseconds}ms");
        if (Prefs.syncEffects) LoggerInstance.Msg(System.ConsoleColor.Blue, $" - Fusion module find & start: {syncSW.ElapsedMilliseconds}ms");
        if (Prefs.enableRemoteVoting) LoggerInstance.Msg(System.ConsoleColor.Blue, $" - Remote voter unpack & start: {botSW.ElapsedMilliseconds}ms");

        #endregion

#if DEBUG
        //UnityWebRequest www = UnityWebRequest.Get("https://extraes.xyz/api/accesscontrol/chaos/auth");
        //var req = www.SendWebRequest();
        //await AsyncUtilities.ToUniTask(req);
        //const long SUCCESS = 200;
        //if (req.webRequest.responseCode != SUCCESS)
        //{
        //    Error("Expected " + SUCCESS + " but got " + req.webRequest.responseCode);
        //    UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
        //}

        dlc.Success();
#endif
    }

    private async Task SetupLatePatching()
    {
        Stopwatch sw = Stopwatch.StartNew();
        while (!GlobalVariables.Player_RigManager)
            await UniTask.Yield();
        sw.Stop();
        LoggerInstance.Msg($"Waited {sw.Elapsed.TotalSeconds:0.00} sec to run patches. Starting now.");

        Stopwatch patchSW = Stopwatch.StartNew();

        // System.Action -> Il2CppSystem.Action (which may be used in patchers) allocates in IL2CPP domain. Task.Run is liable to run in off-main thread, which IL2CPP may not know about.
        // and IL2CPP is like my bitch ex that wants to know everything that's going on. difference is IL2CPP isnt getting the house.
        Utilities.AttachIl2CppToThread();

        //HarmonyInstance.CreateClassProcessor(typeof(thepatch)).Patch();
        // this all happens before the first "await" so it should happen synchronously
        foreach (var type in AsmTypes)
        {
            if (type.IsAbstract || type.IsInterface) continue;
            if (type.GetInterface(nameof(IPatcher)) is null) continue;
            InterfaceMapping patchImpl = type.GetInterfaceMap(typeof(IPatcher));

#if DEBUG
            Log("Executing patcher: " + type.Name);
#endif
            // not worth creating a delegate
            patchImpl.TargetMethods.ForEach(m => m.Invoke(null, Array.Empty<object>()));

        }

        patchSW.Stop();

        LoggerInstance.Msg($"Finished late-patching to avoid crashes. Patching took {patchSW.ElapsedMilliseconds}ms");
    }

    public override void OnApplicationQuit()
    {
        // If they were started, stop the clients and their processes.
        GlobalVariables.WatsonClient?.Stop();
        botProcess?.Kill();
        botProcess?.Dispose();

        IReadOnlyList<MelonPreferences_Category> cats = EffectConfig.rawCategories;
        Log("Saving preferences for " + cats.Count + " effects");
        foreach (MelonPreferences_Category cat in cats) cat.SaveToFile(false);
    }

    // rename OnSceneWasInitialized because BL is built hella different i guess (addressables scene manager on crack i suppose)
    public void InitializeReferences(int buildIdx, string sceneName)
    {
        if (Instances.Player_RigManager == null) return;
#if DEBUG
        string otherName = SceneManager.GetActiveScene().name;
        Chaos.Log($"LOADEDSCENE {sceneName} IDX {buildIdx}, SCENEMANAGERACTIVE {otherName}");
        if (sceneName != "1378bdcaf9526974d98cc23b94c6ab5c" && // Void G114
            sceneName != "scene_GameBootstrap" &&              // OpenXR check
            sceneName != "77da2b1cce998aa4fb4fc76a7fd80e05")
        {
            new TextureSwap().Run();
        }
#endif 

        // you already know what the fuck goin on
        if (EffectHandler.allEffects.Count < 1)
        {
#if DEBUG
            Log("NEED MORE EFFECTS");
#endif
            while (true) { }
        }


#if DEBUG
        Log("Finding scene references!");
        Stopwatch sw = Stopwatch.StartNew();
#endif
        // JeviLib Instances already finds instances
        //todo: test to see if jevilib's onscenewasinitialized runs before chaos's oswi
        GlobalVariables.Player_BodyVitals =
            Instances.Player_BodyVitals;
        GlobalVariables.Player_RigManager =
            Instances.Player_RigManager;
        GlobalVariables.Player_Health =
            Instances.Player_Health;
        GlobalVariables.Player_PhysRig =
            Instances.Player_PhysicsRig;

        Transform pHead = GlobalVariables.Player_PhysRig.m_head;

        GameObject musicPlayer = new GameObject("ChaosMusicPlayer");
        musicPlayer.transform.parent = pHead.transform;
        GlobalVariables.MusicPlayer = musicPlayer.AddComponent<AudioPlayer>();
        GlobalVariables.MusicPlayer._source = musicPlayer.AddComponent<AudioSource>();
        GlobalVariables.MusicPlayer.source.outputAudioMixerGroup = GlobalVariables.MusicMixer;
        GlobalVariables.MusicPlayer._defaultVolume = 0.1f;
        GlobalVariables.MusicPlayer.source.volume = 0.1f;
        GlobalVariables.MusicPlayer.enabled = true;

        GameObject sfxPlayer = new GameObject("ChaosSFXPlayer");
        sfxPlayer.transform.parent = pHead.transform;
        GlobalVariables.SFXPlayer = sfxPlayer.AddComponent<AudioPlayer>();
        GlobalVariables.SFXPlayer._source = sfxPlayer.AddComponent<AudioSource>();
        GlobalVariables.SFXPlayer.source.outputAudioMixerGroup = GlobalVariables.SFXMixer;
        GlobalVariables.SFXPlayer._defaultVolume = 0.25f;
        GlobalVariables.SFXPlayer.source.volume = 0.25f;
        GlobalVariables.SFXPlayer.enabled = true;

        new GameObject("ChaosUIEffectHandler").AddComponent<EffectHandler>();
        //string sceneName = SceneManager.GetActiveScene().name;
        EffectHandler.advanceTimer = sceneName != "1378bdcaf9526974d98cc23b94c6ab5c" && // Void G114
                                     sceneName != "scene_GameBootstrap" &&              // OpenXR check
                                     sceneName != "77da2b1cce998aa4fb4fc76a7fd80e05";   // loading screen

        Stats.PingVersion();
#if DEBUG
        sw.Stop();
        Chaos.Log("Found all globalvar's in " + sw.ElapsedMilliseconds + "ms");
#endif
        Physics.gravity = new Vector3(0, -9.81f, 0);


        // get the effect from effectonsceneload and run it
        if (!EffectHandler.advanceTimer) return;

        if (EffectHandler.allEffects.TryGetValue(Prefs.effectOnSceneLoad, out EffectBase effect))
        {
            Type t = effect.GetType();
            EffectBase e = (EffectBase)Activator.CreateInstance(t)!;
            Chaos.Log($"Running effect '{e.Name}' (from preference) on scene load");
            e.Run();
        }
        else if (!string.IsNullOrWhiteSpace(Prefs.effectOnSceneLoad))
        {
            Chaos.Warn($"{nameof(Prefs.effectOnSceneLoad)} value '{Prefs.effectOnSceneLoad}' wasn't found in the effect dictionary! Check to make sure you matched the spelling and case of the effect name!");

            // check for effects that have the same name but different capitalization, or maybe they left a space at the end
            foreach (string name in EffectHandler.allEffects.Select(e => e.Key.ToLower()))
            {
                if (name == Prefs.effectOnSceneLoad.Trim())
                {
                    Chaos.Warn($"It seems like you were trying to choose '{name}' as the effect to be ran on scene load, but either had whitespace or incorrect capitalization");
                    GUIUtility.systemCopyBuffer = name;
                    Chaos.Log($"Copied '{name}' to your clipboard so you can replace the value in MelonPreferences if you want.");
                }
            }
        }
    }

    public override void OnUpdate()
    {
        if (GlobalVariables.Player_PhysRig)
        {
            GlobalVariables.inFrontOfPlayer = GlobalVariables.Player_PhysRig.m_head.position + GlobalVariables.Player_PhysRig.m_head.forward * 2;
            GlobalVariables.lookingAtPlayer = Quaternion.LookRotation(-GlobalVariables.Player_PhysRig.m_head.forward);
        }

        foreach (EffectBase effect in GlobalVariables.ActiveEffects)
            effect.OnEffectUpdate();
        Extras.WebResponseHandler.Callback(); // bitchass unity doesnt like me doing shit from the websocket thread so here we are
    }

    // If MelonPreferences.cfg is saved while the game is open, make sure the changes are reflected in real time.
    public override void OnPreferencesLoaded()
    {
        // ML 0.5.2 likes to do this cool thing where it calls OnPreferencesLoaded multiple times
        // before it even runs OnApplicationStart, so we need to ML-proof this for some fucking reason.
        if (started) LiveUpdateEffects();
    }

    #region Websocket Methods

    private async void ClientConnectedToServer(object sender, EventArgs e)
    {
        Chaos.Log("Connected to the bot!");
        await GlobalVariables.WatsonClient.SendAsync("ignorerepeatvotes:" + Prefs.ignoreRepeatVotes);
        // Send data for startup then clear it out so that there's less of an opportunity for reflection to steal shit (i think)
        Prefs.SendBotInitalValues(); // doesnt really matter if we await this
    }

    private void ClientDisconnectedFromServer(object sender, EventArgs e)
    {
        Chaos.Log("Disconnected from the voting process. If you didn't close the game, make sure your antivirus isn't killing it");
    }

    private void ClientReceiveMessage(object sender, MessageReceivedEventArgs e)
    {
        string[] splitMessage = Utilities.Argsify(Encoding.UTF8.GetString(e.Data), ':');
        string messageType = splitMessage[0];
        string messageData = splitMessage.Length == 1 ? "" : splitMessage[1];
        switch (messageType)
        {
            case "error":
                Error("An error has occured within the remote voting process!");
                Error(messageData);
                break;
            case "log":
                Chaos.Log(messageData);
                break;
            case "web":
#if DEBUG
                Chaos.Log("Recieved from webserver: " + messageData);
#endif
                Extras.WebResponseHandler.GotData(messageData.Trim());
                break;

            default:
                Error("UNKNOWN MESSAGE TYPE: " + messageType);
                break;
        }
    }

    #endregion

    #region Startup Methods

    private void StartBot()
    {
        Chaos.Log("Unpacking and starting remote voting process...");

        #region Extract Bot

        string saveFolder = Path.Combine(Path.GetTempPath(), "BW-Chaos");
        string exePath = Path.Combine(saveFolder, "BWChaosDiscordBot.exe");

        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
        if (File.Exists(exePath)) File.Delete(exePath);

        using (Stream stream = Assembly.GetManifestResourceStream("BLChaos.Resources.BLChaosDiscordBot.zip")!)
        {
            byte[] buffer = new byte[4096];

            // holy fucking shit i love using
            // no but fr zip the bot because it makes it significantly (~40mb) smaller, even with the dogwater deflate algorithm, and do all this in memory to avoid writing temp files to disk
            throw new NotImplementedException("The Chaos bot client is currently unsupported.");
            // using ZipFile zipFile = new ZipFile(stream);
            // using Stream zipStream = zipFile.GetInputStream(zipFile[0]);
            // using Stream fsOut = File.Create(exePath);
            // StreamUtils.Copy(zipStream, fsOut, buffer);
            // using () using () using () using () using () using () using () using () using () 
        }

        #endregion

        #region Set up server

        botProcess = new Process();
        botProcess.StartInfo.FileName = exePath;
        botProcess.StartInfo.WorkingDirectory = saveFolder;
        botProcess.StartInfo.UseShellExecute = true;
        botProcess.StartInfo.CreateNoWindow = true;
        botProcess.StartInfo.RedirectStandardOutput = true;
        botProcess.StartInfo.RedirectStandardError = true;
        botProcess.OutputDataReceived += BotWritesToStdOut;
        botProcess.ErrorDataReceived += BotWritesToStdErr;
        botProcess.Start();

        // the mod is the client because trying to create a WWS server didnt work, but a client did for some reason. mono moment i guess.
        GlobalVariables.WatsonClient = new WatsonWsClient("127.0.0.1", 8827, false);
        GlobalVariables.WatsonClient.ServerConnected += ClientConnectedToServer;
        GlobalVariables.WatsonClient.ServerDisconnected += ClientDisconnectedFromServer;
        GlobalVariables.WatsonClient.MessageReceived += ClientReceiveMessage;
        GlobalVariables.WatsonClient.Start();

        #endregion
    }

    private void BotWritesToStdOut(object sender, DataReceivedEventArgs e)
    {
        Log("[ChaosBotLog] " + e.Data);
    }

    private void BotWritesToStdErr(object sender, DataReceivedEventArgs e)
    {
        Error("[ChaosBotError] " + e.Data);
    }

    private static void PopulateEffects()
    {
        if (EffectHandler.allEffects.Count == 0)
        {
            // Get all effects from the assembly
            asmEffects = (from t in AsmTypes
                          where t.IsSubclassOf(typeof(EffectBase))
                            && t != typeof(StickDrift)
#if DEBUG
                            && !t.CustomAttributes.Any(a => a.AttributeType == typeof(DontRegisterEffect)) // DontRegisterEffect's shouldnt even be pressent in release builds
#endif
                          select (EffectBase)Activator.CreateInstance(t)).ToList();
        }
        else
        {
            EffectHandler.allEffects.Clear();
            EffectHandler.bag.Clear();
            Prefs.Get();
        }

        // Actually populate the effects list
        foreach (EffectBase e in FilterEffects(asmEffects))
        {
            if (!Prefs.ForceDisabledEffects.Contains(e.Name)) EffectHandler.allEffects.Add(e.Name, e);
#if DEBUG
            else Chaos.Log($"{nameof(Prefs.ForceDisabledEffects)} has this effect {e.Name}, refusing to add it"); // haha nameof nameof nameof nameof nameof nameof nameof
#endif
        }

#if DEBUG
        Chaos.Log($"{nameof(Prefs.ForceDisabledEffects)} has {Prefs.ForceDisabledEffects.Count} things");
        foreach (string disabled in Prefs.ForceDisabledEffects)
        {
            Chaos.Log($" - {disabled}");
        }
#endif

        foreach (string str in Prefs.ForceEnabledEffects)
        {
#if DEBUG
            Chaos.Log("Force enabling effect '" + str + "' because it was in the melonprefs array");
#endif

            if (EffectHandler.allEffects.Keys.Contains(str)) continue; // we dont want it in the list twice

            EffectBase effect = asmEffects.FirstOrDefault(e => e.Name == str); // firstordefault my beloved

            // If the effect name doesn't exist, "throw" an error
            if (effect == null)
            {
                Chaos.Warn($"Force enabled effect '{str}' wasn't found! Check MelonPreferences, are you sure that's the right name for the effect?");
                continue;
            }

            // don't allow oculus players to try and crash my shit
            if (effect.Types.HasFlag(EffectTypes.USE_STEAM) && !isSteamVer)
            {
                Chaos.Warn("This is the Oculus version, however you attempted to use a Steam effect! This is not allowed! Are you trying to crash your game???");
                continue;
            }

            EffectHandler.allEffects.Add(str, effect);
        }

        #region Local function because fuck you

        static IEnumerable<EffectBase> FilterEffects(IEnumerable<EffectBase> effects)
        {
            return from e in effects
                   where e.Types == EffectTypes.NONE || // is this optimization?
                   IsEffectAllowed(e.Types)
                   select e;
        }

        #endregion
    }

    internal static bool IsEffectAllowed(EffectTypes eTypes)
    {
        foreach ((EffectTypes type, bool allowed) in eTypesToPrefs)
            if (eTypes.HasFlag(type) && !allowed) return false; //todid: this fucking works?????
        return true;
    }

    internal static void LiveUpdateEffects()
    {
        // I'm not sure what this would do, but it probably doesn't hurt...
        if (!EffectHandler.Instance == null) EffectHandler.Instance.gameObject.SetActive(false);
        PopulateEffects();
        foreach (EffectBase e in GlobalVariables.ActiveEffects.Where(e => !IsEffectAllowed(e.Types))) e.ForceEnd(); // linqlinqlinqlinqlinqlinqlinqlinq
        EffectHandler.CopyAllToBag();
        if (!EffectHandler.Instance == null) EffectHandler.Instance.gameObject.SetActive(true);
    }

    #endregion

    public static void InjectEffect<T>() where T : EffectBase
    {
        InjectEffect(typeof(T));
    }

    public static void InjectEffect(Type type)
    {
        if (type.BaseType != typeof(EffectBase)) throw new InvalidOperationException($"Supplied type {type.Name} does not extend {nameof(EffectBase)} - it must do so in order to be injected into Chaos");
        EffectBase e = (EffectBase)Activator.CreateInstance(type);
#if DEBUG
        Chaos.Log($"Injecting effect {e.Name} (type {type.Name}) into the effect collections");
#endif
        asmEffects.Add(e);
        if (IsEffectAllowed(e.Types))
        {
            EffectHandler.allEffects.Add(e.Name, e);
            EffectHandler.bag.Add(e.Name, e);
        }
        if (Instance.started) e.RegisterPreferences();
    }

    internal static void DispatchEffectRan(EffectBase effect) => Chaos.OnEffectRan?.InvokeSafeSync(effect);

    #region MelonLogger replacements

    internal static void Log(string str) => Instance.LoggerInstance.Msg(str);
    internal static void Log(object obj) => Instance.LoggerInstance.Msg(obj?.ToString() ?? "null");
    internal static void Warn(string str) => Instance.LoggerInstance.Warning(str);
    internal static void Warn(object obj) => Instance.LoggerInstance.Warning(obj?.ToString() ?? "null");
    internal static void Error(string str) => Instance.LoggerInstance.Error(str);
    internal static void Error(object obj) => Instance.LoggerInstance.Error(obj?.ToString() ?? "null");
    internal static void Error(string str, Exception ex) => Instance.LoggerInstance.Error(str, ex);

    #endregion
}