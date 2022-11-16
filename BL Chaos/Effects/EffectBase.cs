using Jevil.IMGUI;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BLChaos.Effects;

// todo: maybe add a "conflicting effects" list variable in case of something such as 2 effects modifying gravity
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
        DEFAULT_DISABLED = 1 << 7,
    }

    public enum NetMsgType : byte
    {
        STRING = 0,
        BYTEARRAY = 1,
        RAWBYTES = 2,
        START = 3,
    }

    public string Name { get; }
    public int Duration { get; }
    public EffectTypes Types { get; }
    //private static readonly Dictionary<string, MenuCategory> elements = new Dictionary<string, MenuCategory>(); // Allow effects to view their own menu elements. Why? Not sure, custom melonprefs maybe.
    //public MenuCategory MenuElement
    //{
    //    get { return elements[GetType().Name]; }
    //    set { elements[GetType().Name] = value; }
    //}

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

    private MethodInfo FindAutoCR()
    {
        // LINQLINQLINQLINQLINQMYBELOVEDLINQLINQLINQLINQLINQILOVELINQLINQLINQLINQLINQLINQLINQLINQLINQLINQ
        return (from method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                where method.ReturnType == typeof(IEnumerator) &&
                      method.GetCustomAttribute<AutoCoroutine>() != null
                select method).FirstOrDefault();
        // I LOVE LINQ YEAHHHHHHHHH LINQLINQLINQLINQLINQMYBELOVEDLINQLINQLINQLINQLINQILOVELINQLINQLINQLINQ
    }

    // gets called from BoneMenu.cs after the effect's subcategory has been created and set up
    public void GetPreferencesFromAttrs()
    {
        Type myType = GetType();

#if DEBUG
        FieldInfo[] instanceFields = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        IEnumerable<FieldInfo> instancePrefs = instanceFields.Where(f => f.GetCustomAttribute<EffectPreference>() != null);
        if (instancePrefs.Count() != 0)
        {
            Chaos.Warn($"This effect {Name} ({myType.Name}) declares instanced preferences, this is not allowed!");
            Chaos.Warn($"These preferences are: ");
            foreach (FieldInfo field in instancePrefs) Chaos.Warn(" - " + field.Name);
        }
#endif

        FieldInfo[] staticFields = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        if (staticFields.Length == 0) return;

        foreach (FieldInfo field in staticFields)
        {
            EffectPreference ep = field.GetCustomAttribute<EffectPreference>();
            if (ep == null) continue;
#if DEBUG
            Chaos.Log($"Found effect preference on {myType.Name}.{field.Name}");
#endif
            chaosConfigCategory = chaosConfigCategory ?? EffectConfig.GetOrCreateCategory(myType.Name);

            Type type = field.FieldType;
            string readableName = Utilities.GetReadableStringFromMemberName(field.Name);
            if (type == typeof(string))
            {
                MelonPreferences_Entry<string> entry = SetEntry(field, out string toSet, ep.desc);
                //MenuElement.CreateStringElement(readableName, Color.white, toSet, val => { field.SetValue(null, val); entry.Value = val; chaosConfigCategory.SaveToFile(false); });
            }
            else if (type == typeof(bool))
            {
                MelonPreferences_Entry<bool> entry = SetEntry(field, out bool toSet, ep.desc);
                //MenuElement.CreateBoolElement(readableName, Color.white, toSet, val => { field.SetValue(null, val); entry.Value = val; chaosConfigCategory.SaveToFile(false); });
            }
            else if (type == typeof(Color))
            {
                MelonPreferences_Entry<Color> entry = SetEntry(field, out Color toSet, ep.desc);
                //MenuElement.CreateColorElement(readableName, toSet, val => { field.SetValue(null, val); entry.Value = val; chaosConfigCategory.SaveToFile(false); });
            }
            else if (type.IsEnum)
            {
#if DEBUG
                if (!string.IsNullOrEmpty(ep.desc)) Chaos.Warn($"Descriptions are not allowed for enum preferences - the description '{ep.desc}' on {myType.Name}.{field.Name} will not be put in ChaosConfig.cfg");
#endif
                Enum dv = (Enum)field.GetValue(null);
                MelonPreferences_Entry<string> entry = chaosConfigCategory.CreateEntry<string>(field.Name, dv.ToString(), description: $"Options: {string.Join(", ", Enum.GetNames(type))}");
                Enum toSet = dv; // if the two are different
                try
                {
                    toSet = (Enum)Enum.Parse(type, entry.Value);
                }
                catch
                {
                    Chaos.Warn($"Failed to parse '{entry.Value}' as a value in the enum {type.Name} for the effect {myType.Name}'s preference {field.Name}");
                    Chaos.Warn("Replacing it with its default value of " + dv);
                    entry.Value = dv.ToString();
                }
                //MenuElement.CreateEnumElement(readableName, Color.white, toSet, val => { field.SetValue(null, val); entry.Value = val.ToString(); chaosConfigCategory.SaveToFile(false); });
            }
#if DEBUG
            else
            {
                Chaos.Error($"{myType.Name}.{field.Name} is of un-bonemenu-able (or un-melonpreferences-able) type {type.Name}! This is no good!");
            }
#endif
        }


#if DEBUG

        IEnumerable<FieldInfo> instanceRanges = instanceFields.Where(f => f.GetCustomAttribute<RangePreference>() != null);
        if (instanceRanges.Count() != 0)
        {
            Chaos.Warn($"This effect {Name} ({myType.Name}) declares instanced range preferences, this is not allowed!");
            Chaos.Warn($"These preferences are: ");
            foreach (FieldInfo field in instanceRanges) Chaos.Warn(" - " + field.Name);
        }
#endif

        chaosConfigCategory = chaosConfigCategory ?? EffectConfig.GetOrCreateCategory(myType.Name);
        foreach (FieldInfo field in staticFields)
        {
            RangePreference rp = field.GetCustomAttribute<RangePreference>();
            if (rp == null) continue;
#if DEBUG
            Chaos.Log($"Found effect range preference on {myType.Name}.{field.Name}");
#endif
            chaosConfigCategory = chaosConfigCategory ?? EffectConfig.GetOrCreateCategory(myType.Name);

            string readableName = Utilities.GetReadableStringFromMemberName(field.Name);
            object defaultValue = field.GetValue(null);
            if (field.FieldType == typeof(int))
            {
                MelonPreferences_Entry<int> entry = SetEntry(field, out int toSet, $"{rp.low} to {rp.high}");
                //MenuElement.CreateIntElement(readableName, Color.white, toSet, val => { field.SetValue(null, val); entry.Value = val; chaosConfigCategory.SaveToFile(false); }, (int)rp.inc, (int)rp.low, (int)rp.high);
            }
            else if (field.FieldType == typeof(float))
            {
                MelonPreferences_Entry<float> entry = SetEntry(field, out float toSet, $"{rp.low} to {rp.high}");
                //MenuElement.CreateFloatElement(readableName, Color.white, toSet, val => { field.SetValue(null, val); entry.Value = val; chaosConfigCategory.SaveToFile(false); }, rp.inc, rp.low, rp.high);
            }
#if DEBUG
            else
            {
                Chaos.Error($"{myType.Name}.{field.Name} is of un-range-able type {field.FieldType.Name}! This is no good!");
            }
            Chaos.Log("Successfully created range preference");
#endif
        }

#if DEBUG
        Chaos.Log($"Created {chaosConfigCategory.Entries.Count} entries in the preference category for {Name} ({myType.Name})");
#endif
        if (chaosConfigCategory.Entries.Count != 0) chaosConfigCategory.SaveToFile(false);
    }

    private MelonPreferences_Entry<T> SetEntry<T>(FieldInfo field, out T toSet, string desc = "")
    {
        T dv = (T)field.GetValue(null);
        desc = string.Join(", ", "Default: " + dv.ToString(), desc);
        MelonPreferences_Entry<T> entry = chaosConfigCategory.CreateEntry<T>(field.Name, dv, description: desc);
        T ev = entry.Value;
        toSet = dv.Equals(ev) ? ev : ev; // if the two are different
        return entry;
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

    protected void Log(UnityEngine.Object obj) => Log(obj != null ? $"<{obj.GetType().Name}> {obj.name} '{obj.ToString()}'" : "<null>");
    protected void Log(object obj) => Log(obj != null ? obj.ToString() : "<null>");
    protected void Log(string str) => Chaos.Log($"-> [{Name}] " + str);

    public void Run()
    {
#if DEBUG
        // If there's already an instance of this effect, abort immediately, the new effect system creates a new instance when ran. only IMGUI uses this, so it shouldnt be possible under normal circumstances
        if (EffectHandler.allEffects.Values.Contains(this))
        {
            Chaos.Warn("The effect handler has an instance of this effect! This should not happen! Are you using IMGUI? Creating a new instance, running, then aborting!");
            EffectBase newE = (EffectBase)Activator.CreateInstance(GetType());
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
        Chaos.OnEffectRan?.Invoke(this);
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

        Log($"Waiting {Duration} SCALED seconds before ending");
        while (Time.realtimeSinceStartup - StartTime < Duration)
        {
            yield return null;
            Log($"RTSS={Time.realtimeSinceStartup:F2};Dur={Duration};RUNTIME={Time.realtimeSinceStartup - StartTime:F2};CONT?={Time.realtimeSinceStartup - StartTime < Duration}");
        }
        Log($"Done waiting {Duration} SCALED seconds");
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
                    HandleNetworkMessage(Encoding.ASCII.GetString(data));
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
        _sendData?.Invoke(NetMsgType.STRING, myIndex, Encoding.ASCII.GetBytes(data));
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
        Chaos.Log($"Effect {Name} is sending {data.Length} bytes");
#endif

        _sendData?.Invoke(NetMsgType.RAWBYTES, myIndex, data);
    }

    private IEnumerator CoHookNetworker()
    {
        _dataRecieved += FilterNetworkData;
        yield return new WaitForSecondsRealtime(5);
        _dataRecieved -= FilterNetworkData;
    }

    private void AddToPrevEffects()
    {
        if (GlobalVariables.PreviousEffects.Count >= 7)
            GlobalVariables.PreviousEffects.RemoveAt(0);
        GlobalVariables.PreviousEffects.Add(Name);
    }
}
