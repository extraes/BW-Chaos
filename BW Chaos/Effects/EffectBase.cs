using MelonLoader;
using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Linq;
using BWChaos.Extras;
using System;

namespace BWChaos.Effects
{
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
        }

        public string Name { get; }
        public int Duration { get; }
        public EffectTypes Types { get; }
        public ModThatIsNotMod.BoneMenu.MenuElement MenuElement { get; set; } // Allow effects to view their own menu elements. Why? Not sure, custom melonprefs maybe.

        public bool Active { get; private set; }
        public float StartTime { get; private set; }
        public static Action<string, string> _dataRecieved; // for internal use by the base class; format is (name, data); both are strings instead of bytes because fuck you its easier that way
        public static Action<string, string> _sendData; // for internal use by base class and sync handler
        public bool isNetworked = false;
        public object autoCRToken;
        private readonly MethodInfo autoCRMethod;

        private object coRunToken;
        private bool hasFinished;

        public EffectBase(string eName, int eDuration, EffectTypes eTypes = EffectTypes.NONE)
        {
            Name = eName;
            Duration = eDuration;
            Types = eTypes;
            // LINQLINQLINQLINQLINQMYBELOVEDLINQLINQLINQLINQLINQILOVELINQLINQLINQLINQLINQLINQLINQLINQLINQLINQ
            autoCRMethod = (from method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                           where method.ReturnType == typeof(IEnumerator) &&
                                 method.GetCustomAttribute<AutoCoroutine>() != null
                           select method).FirstOrDefault();
        }
        
        public EffectBase(string eName, EffectTypes eTypes = EffectTypes.NONE)
        {
            Name = eName;
            Duration = 0;
            Types = eTypes;
#if DEBUG
            if ((from method in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
             where method.ReturnType == typeof(IEnumerator) &&
                   method.GetCustomAttribute<AutoCoroutine>() != null
             select method).FirstOrDefault() != null) Chaos.Warn("Effect " + Name + " is supposed to be a one off but it has an AutoCR! Did you mean to give it a duration in the constructor?");
#endif
        }

#if DEBUG
        ~EffectBase()
        {
            Chaos.Log("I just learned about finalizers soooo");
            Chaos.Log(Name + " went out of scope/was GC'd");
        }
#endif

        public virtual void HandleNetworkMessage(string data) { Chaos.Warn($"This effect '{Name}' sends data but it doesn't receive it! Why?"); } // make sure i dont get caught lacking
        public virtual void OnEffectStart() { }
        public virtual void OnEffectUpdate() { }
        public virtual void OnEffectEnd() { }

        public void Run()
        {
#if DEBUG
            // If there's already an instance of this effect, abort immediately, the new effect system creates a new instance when ran. only IMGUI uses this, so it shouldnt be possible under normal circumstances
            if (EffectHandler.AllEffects.Values.Contains(this))
            {
                Chaos.Warn("The effect handler has an instance of this effect! This should not happen! Are you using IMGUI? Creating a new instance, running, then aborting!");
                var newE = (EffectBase)Activator.CreateInstance(this.GetType());
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
            if (autoCRMethod != null) autoCRToken = MelonCoroutines.Start((IEnumerator)autoCRMethod.Invoke(this, null));
            _dataRecieved += FilterNetworkData;
            OnEffectStart();

            Active = true;
            StartTime = Time.realtimeSinceStartup;
            GlobalVariables.ActiveEffects.Add(this);

            yield return new WaitForSecondsRealtime(Duration);

            GlobalVariables.ActiveEffects.Remove(this);
            Active = false;

            if (autoCRToken != null) MelonCoroutines.Stop(autoCRToken);
            _dataRecieved -= FilterNetworkData;
            OnEffectEnd();
            hasFinished = true;
            AddToPrevEffects();
#if DEBUG
            Chaos.Log(Name + " has finished running");
#endif
        }

        private void FilterNetworkData(string name, string data)
        {
#if DEBUG
            Chaos.Log($"Recieved effect data destined for '{name}' -> {data}");
#endif
            if (name == Name) HandleNetworkMessage(data);
        }

        protected void SendNetworkData(string data)
        {
#if DEBUG
            Chaos.Log($"Effect {Name} is sending data {data}");
#endif
            _sendData?.Invoke(Name, data);
        }
        
        private IEnumerator CoHookNetworker()
        {
            _dataRecieved += FilterNetworkData;
            yield return new WaitForSecondsRealtime(5); // let effects send and recieve data for 5 seconds
            _dataRecieved -= FilterNetworkData;
        }

        private void AddToPrevEffects()
        {
            if (GlobalVariables.PreviousEffects.Count >= 7)
                GlobalVariables.PreviousEffects.RemoveAt(0);
            GlobalVariables.PreviousEffects.Add(Name);
        }
    }
}
