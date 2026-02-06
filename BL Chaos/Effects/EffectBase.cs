using Jevil.IMGUI;
using Jevil.Prefs;
using MelonLoader;
using MelonLoader.Utils;
using System.Reflection;

#if !NOBONELIB
using BoneLib.BoneMenu;
#endif

namespace BLChaos.Effects;

// todo: maybe add a "conflicting effects" list variable in case of something such as 2 effects modifying gravity
[PreferencesFile("./ChaosConfig.cfg")]
public class EffectBase
{
    [Flags]
    public enum EffectTypes
    {
        NONE = 0,
        AFFECT_GRAVITY = 1 << 0,
        AFFECT_STEAM_PROFILE = 1 << 1,
        USE_STEAM = 1 << 2,
        LAGGY = 1 << 3,
        HIDDEN = 1 << 4,
        DONT_SYNC = 1 << 5,
        META = 1 << 6,
        POST_PROCESS = 1 << 7,
        POST_PROCESS_ANIMATED = 1 << 8,
        DEFAULT_DISABLED = 1 << 9,
    }

    public enum NetMsgType : byte
    {
        STRING = 0,
        BYTEARRAY = 1,
        RAWBYTES = 2,
        START = 3,
    }

#if DEBUG
    protected internal enum TestResult
    {
        NOT_IMPLEMENTED = -1,
        INCONCLUSIVE = 0,
        USER_INTERVENTION_NEEDED = 1,
        FAILURE = 2,
        SUCCESS = 3,
    }

    protected TestResult Res(bool? res) => res switch
    {
        true => TestResult.SUCCESS,
        false => TestResult.FAILURE,
        _ => TestResult.INCONCLUSIVE,
    };
    protected Task<TestResult> ResT(bool? res) => Task.FromResult(Res(res));
#endif

    public string Name { get; }
    public int Duration { get; }
    public EffectTypes Types { get; }
#if !NOBONELIB
    private static readonly Dictionary<string, Page> elements = new Dictionary<string, Page>(); // Allow effects to view their own menu elements. Why? Not sure, custom melonprefs maybe.
    public Page Page
    {
        get { return elements[GetType().Name]; }
        set { elements[GetType().Name] = value; }
    }
#endif

    public bool Active { get; private set; }
    public float StartTime { get; private set; }
    // https://stackoverflow.com/questions/5851497/static-fields-in-a-base-class-and-derived-classes
    private static readonly Dictionary<string, byte> indices = new Dictionary<string, byte>();
    private static byte nextIndex;
    public byte EffectIndex
    {
        get { return indices[GetType().Name]; }
        set { indices[GetType().Name] = value; }
    }
    private byte myIndex; // cache as a byte so i dont have GetType called every time i access it

    /// <summary>
    /// Fired by Entanglement module and used by base class to fire override. The format is (type, effect index, data)
    /// </summary>
    public static Action<NetMsgType, byte, byte[]> _dataRecieved;
    /// <summary>
    /// Fired by base class. Use SendNetworkData instead.
    /// Format is (type, index, data)
    /// </summary>
    public static Action<NetMsgType, byte, byte[]> _sendData; // for internal use by base class and sync handler
    public bool isNetworked = false;
    private object autoCRToken;
    private readonly MethodInfo autoCRMethod;
    private MelonPreferences_Category chaosConfigCategory;

    private object coRunToken;
    private bool hasFinished;

    public EffectBase(string eName, int eDuration, EffectTypes eTypes = EffectTypes.NONE)
    {
        Name = eName;
        Duration = eDuration;
        Types = eTypes;

        autoCRMethod = FindAutoCR();
    }

    public EffectBase(string eName, EffectTypes eTypes = EffectTypes.NONE)
    {
        Name = eName;
        Duration = 0;
        Types = eTypes;

#if DEBUG
        if (FindAutoCR() != null) Chaos.Warn($"Effect {Name} ({GetType().Name}) is supposed to be a one off but it has an AutoCR! Did you mean to give it a duration in the constructor?");
#endif
    }

    private MethodInfo? FindAutoCR()
    {
        // LINQLINQLINQLINQLINQMYBELOVEDLINQLINQLINQLINQLINQILOVELINQLINQLINQLINQLINQLINQLINQLINQLINQLINQ
        return (from method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.ReturnType == typeof(IEnumerator) &&
                      method.GetCustomAttribute<AutoCoroutine>() != null
                select method).FirstOrDefault();
        // I LOVE LINQ YEAHHHHHHHHH LINQLINQLINQLINQLINQMYBELOVEDLINQLINQLINQLINQLINQILOVELINQLINQLINQLINQ
    }

    // gets called from BoneMenu.cs after the effect's subcategory has been created and set up
    public void RegisterPreferences()
    {
        MelonPreferences_Category myMpCategory = MelonPreferences.CreateCategory(Name);
        myMpCategory.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "ChaosConfig.cfg"), true, false);
#if !NOBONELIB
        Page myPage = BoneMenu.GetPageFor(Name);
        Preferences.RegisterUnder(GetType(), myMpCategory, myPage);
#endif
    }

    private void GetIndex()
    {

        if (!indices.ContainsKey(GetType().Name))
        {
#if DEBUG
            Chaos.Log($"Gave type {GetType().Name} new index {nextIndex + 1}");
#endif
            EffectIndex = nextIndex++;
        }
#if DEBUG
        else Chaos.Log($"Created new instance of {GetType().Name} with preexisting index {EffectIndex}");
#endif
        myIndex = EffectIndex;
    }

#if DEBUG
    ~EffectBase()
    {
        Chaos.Log("I just learned about finalizers soooo");
        Chaos.Log(Name + " went out of scope/was GC'd");
    }
#endif

    // you may think "extraes, its excessive to have this many overrides!"
    // but i think this makes it easier to program. unironically
    // because you can send a string and recieve it through an override and do the same with bytes
    // like sending an obj name and then its position, because its horribly inefficient to send positions as strings
    // so you can take the string and the position and assume it gets taken normally
    // because its sorted in the "backend" of the base class
    public virtual void HandleNetworkMessage(string data) { Chaos.Warn($"This effect '{Name}' sends string data but it doesn't receive it! Why?"); } // make sure i dont get caught lacking
    public virtual void HandleNetworkMessage(byte[] data) { Chaos.Warn($"This effect '{Name}' sends bytes but it doesn't receive it! Why?"); } // make sure i dont get caught lacking
    public virtual void HandleNetworkMessage(byte[][] data) { Chaos.Warn($"This effect '{Name}' sends bytes arrays but it doesn't receive it! Why?"); } // make sure i dont get caught lacking
    public virtual void OnEffectStart() { }
    public virtual void OnEffectUpdate() { }
    public virtual void OnEffectEnd() { }

#if DEBUG
    internal virtual Task<TestResult> Test() { return Task.FromResult<TestResult>(default); }
#endif

    protected void Log(UnityEngine.Object obj) => Log(obj != null ? $"<{obj.GetType().Name}> {obj.name} '{obj.ToString()}'" : "<null>");
    protected void Log(object obj) => Log(obj != null ? obj.ToString()! : "<null>");
    protected void Log(string str) => Chaos.Log($"-> [{Name}] {str}");
    protected void LogIfErrored(Exception? ex)
    {
        if (ex is not null)
            Chaos.Error($"-> [{Name}] [ERROR] {ex}");
    }

    public void Run()
    {
#if DEBUG
        // If there's already an instance of this effect, abort immediately, the new effect system creates a new instance when ran. only IMGUI uses this, so it shouldnt be possible under normal circumstances
        if (EffectHandler.allEffects.ContainsValue(this))
        {
            Chaos.Warn("The effect handler has an instance of this effect! This should not happen! Are you using IMGUI? Creating a new instance, running, then aborting!");
            EffectBase newE = (EffectBase)Activator.CreateInstance(GetType())!;
            newE.Run();
            return;
        }

        // Logging for my debug :)
        Chaos.Log("Running effect " + Name + (Duration == 0 ? ", it is a one-off" : "") + (autoCRMethod != null ? ", it has an AutoCoroutine named " + autoCRMethod.Name : ""));
        // in case i forget to give durations to effects
        if (GetType().GetMethod(nameof(OnEffectEnd), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly) != null && Duration == 0)
            Chaos.Warn("Effect " + Name + " is SUPPOSED to be a one off, yet it has an OnEffectEnd! What gives?");
#endif

        if (autoCRToken != null) MelonCoroutines.Stop(autoCRToken); // there can only be one autocr at a time
        Chaos.DispatchEffectRan(this);
        if (Duration == 0)
        {
            OnEffectStart();
            // let one-off effects send and recieve data for 5 seconds because otherwise they wont get to send anything
            MelonCoroutines.Start(CoHookNetworker());
            AddToPrevEffects();
        }
        else coRunToken = MelonCoroutines.Start(CoRun());
    }

    public void ForceEnd()
    {
        if (!hasFinished)
        {
            if (autoCRToken != null) MelonCoroutines.Stop(autoCRToken);
            if (coRunToken != null) MelonCoroutines.Stop(coRunToken);
            LogIfErrored(Utilities.Try(OnEffectEnd));
            try { GlobalVariables.ActiveEffects.Remove(this); } catch { }
            Active = false;
            hasFinished = true;
        }
    }

    private IEnumerator CoRun()
    {
#if DEBUG
        GUIToken token = DebugDraw.TrackVariable("ACTIVE:" + Name, GUIPosition.TOP_RIGHT, () => (StartTime + Duration) - Time.realtimeSinceStartup);
#endif
        if (autoCRMethod != null) autoCRToken = MelonCoroutines.Start((IEnumerator)autoCRMethod.Invoke(this, null));
        _dataRecieved += FilterNetworkData;
        OnEffectStart();

        Active = true;
        StartTime = Time.realtimeSinceStartup;
        GlobalVariables.ActiveEffects.Add(this);

        while (Time.realtimeSinceStartup - StartTime < Duration)
        {
            yield return null;
        }
        GlobalVariables.ActiveEffects.Remove(this);
        Active = false;

        if (autoCRToken != null) MelonCoroutines.Stop(autoCRToken);
        _dataRecieved -= FilterNetworkData;
        OnEffectEnd();
        hasFinished = true;
        AddToPrevEffects();
#if DEBUG
        Log("Finished running");
        DebugDraw.Dont(token);
#endif
    }

    private void FilterNetworkData(NetMsgType type, byte idx, byte[] data)
    {
#if DEBUG
        Chaos.Log($"Recieved a {type} message with {data.Length} bytes destined for effect with index {idx}");
#endif
        if (idx == myIndex)
        {
            switch (type)
            {
                case NetMsgType.STRING:
                    HandleNetworkMessage(Encoding.UTF8.GetString(data));
                    break;
                case NetMsgType.BYTEARRAY:
                    HandleNetworkMessage(Utilities.SplitBytes(data));
                    break;
                case NetMsgType.RAWBYTES:
                    HandleNetworkMessage(data);
                    break;
#if DEBUG
                case NetMsgType.START:
                    Chaos.Warn($"Recieved {nameof(NetMsgType)}.{nameof(NetMsgType.START)}! This should not be possible! Recheck the Entangle module!");
                    break;
#endif
                default:
                    Chaos.Warn($"Unrecognized {nameof(NetMsgType)}: {type}");
                    break;
            }
        }
    }

    protected void SendNetworkData(string data)
    {
#if DEBUG
        Chaos.Log($"Effect {Name} is sending string data - '{data}'");
#endif
        _sendData?.Invoke(NetMsgType.STRING, myIndex, Encoding.UTF8.GetBytes(data));
    }

    protected void SendNetworkData(params byte[][] data)
    {
#if DEBUG
        Chaos.Log($"Effect {Name} is sending data {data.Length} byte arrays");
#endif

        // for the sake of reducing complexity, let me send and recieve byte arrays separated
        byte[] bytesJoined = Utilities.JoinBytes(data);

        _sendData?.Invoke(NetMsgType.BYTEARRAY, myIndex, bytesJoined);
    }

    /// <summary>
    /// This is the only method to get a summary because using bytes in this way is playing with fire. Do not use the 255/0xFF byte.
    /// Debug builds have a check for 255.
    /// </summary>
    /// <param name="data"></param>
    protected void SendNetworkData(byte[] data)
    {
#if DEBUG
        Log($"Sending {data.Length} bytes of data");
#endif

        _sendData?.Invoke(NetMsgType.RAWBYTES, myIndex, data);
    }

    private IEnumerator CoHookNetworker()
    {
        _dataRecieved += FilterNetworkData;
        float realtime = 0;
        while ((realtime += Time.unscaledDeltaTime) < 5) yield return null;
        _dataRecieved -= FilterNetworkData;
    }

    private void AddToPrevEffects()
    {
        if (GlobalVariables.PreviousEffects.Count >= 7)
            GlobalVariables.PreviousEffects.RemoveAt(0);
        GlobalVariables.PreviousEffects.Add(Name);
    }
}
