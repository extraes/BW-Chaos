using MelonLoader;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects
{
    // todo: maybe add a "conflicting effects" list variable in case of something such as 2 effects modifying gravity
    internal class EffectBase
    {
        //todo: enum
        [System.Flags]
        public enum EffectTypes
        {
            NONE = 0, //todo: find out smarter way to do this
            AFFECT_GRAVITY = 1 << 0,
            AFFECT_STEAM_PROFILE = 1 << 1,
            USE_STEAM = 1 << 2,
            LAGGY = 1 << 3,
            HIDDEN = 1 << 4,
        }

        public string Name { get; }
        public int Duration { get; }
        public EffectTypes Types { get; }

        public bool Active { get; private set; }

        private IEnumerator CoRunEnumerator;
        private bool hasFinished;

        public EffectBase(string eName, int eDuration, EffectTypes eTypes = EffectTypes.NONE)
        {
            Name = eName;
            Duration = eDuration;
            Types = eTypes;
        }

        public EffectBase(string eName, EffectTypes eTypes = EffectTypes.NONE)
        {
            Name = eName;
            Duration = 0;
            Types = eTypes;
        }

        public virtual void OnEffectStart() { }
        public virtual void OnEffectUpdate() { }
        public virtual void OnEffectEnd() { }

        public void Run()
        {
            if (Duration == 0) OnEffectStart();
            else CoRunEnumerator = (IEnumerator)MelonCoroutines.Start(CoRun());

            AddToPrevEffects();
        }

        public void ForceEnd()
        {
            if (!hasFinished)
            {
                MelonCoroutines.Stop(CoRunEnumerator);
                try { GlobalVariables.ActiveEffects.Remove(this); } catch { }
                Active = false;
            }
        }

        private IEnumerator CoRun()
        {
            OnEffectStart();

            Active = true;
            GlobalVariables.ActiveEffects.Add(this);

            yield return new WaitForSecondsRealtime(Duration);

            GlobalVariables.ActiveEffects.Remove(this);
            Active = false;

            OnEffectEnd();
            hasFinished = true;
        }

        private void AddToPrevEffects()
        {
            if (GlobalVariables.PreviousEffects.Count >= 7)
                GlobalVariables.PreviousEffects.RemoveAt(0);
            GlobalVariables.PreviousEffects.Add(this);
        }
    }
}
