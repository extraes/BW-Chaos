using BW_Chaos;
using UnityEngine;
using MelonLoader;
using System.Collections;

namespace BW_Chaos.Effects
{
    // todo: maybe add a "conflicting effects" list variable in case of something such as 2 effects modifying gravity
    internal class EffectBase
    {
        public string Name;
        public int Duration;

        public bool active;

        public EffectBase(string eName, int eDuration)
        {
            Name = eName;
            Duration = eDuration;
        }

        public EffectBase(string eName)
        {
            Name = eName;
            Duration = 0;
        }

        public virtual void OnEffectStart() { }
        public virtual void OnEffectUpdate() { }
        public virtual void OnEffectEnd() { }

        public void Run()
        {
            if (Duration == 0) OnEffectStart();
            else MelonCoroutines.Start(CoRun());
        }

        private IEnumerator CoRun()
        {
            // todo: does this actually work, cause i heavily doubt it
            active = true;
            GlobalVariables.ActiveEffects.Add(this);

            OnEffectStart();

            yield return new WaitForSecondsRealtime(Duration);

            GlobalVariables.ActiveEffects.Remove(this);
            active = false;

            OnEffectEnd();
        }
    }
}
