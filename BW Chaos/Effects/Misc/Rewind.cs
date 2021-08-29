using Chronos;
using MelonLoader;
using StressLevelZero.Rig;
using System.Collections;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class Rewind : EffectBase
    {
        public Rewind() : base("Record then rewind 15 seconds", 30) { }
        private GlobalClock clock = null;
        private bool isRewindAlreadyActive = false; // We don't want two rewinds at the same time. That would be bad.
        public override void OnEffectStart()
        {
            if (isRewindAlreadyActive)
            {
                MelonLogger.Warning("Two instances of Rewind can't be active at the same time! Not running again!");
            }

            isRewindAlreadyActive = true;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = false;
            #region Set up Chronos & clock
            GameObject chronos = new GameObject("ChronosController");
            clock = chronos.AddComponent<GlobalClock>();
            clock.key = "Root";
            if (Timekeeper.instance == null) chronos.AddComponent<Timekeeper>();
            #endregion

            #region Register Rb's into clock
            foreach (Rigidbody rb in Resources.FindObjectsOfTypeAll<Rigidbody>())
            {
                if (rb.GetComponentInParent<RigManager>() || rb.IsSleeping() || rb.isKinematic) continue;

                // Ignore rigidbodies that are a part of the body
                Timeline timeline = rb.GetComponent<Timeline>();
                if (timeline == null)
                    timeline = rb.gameObject.AddComponent<Timeline>();
                else
                    continue;
                timeline.mode = TimelineMode.Global;
                timeline._clock = clock;
                timeline.rewindable = true;
                timeline.recordingDuration = 15f;
                clock.Register(timeline);
            }
            #endregion

            #region Register animators into clock
            foreach (Animator anim in Resources.FindObjectsOfTypeAll<Animator>())
            {
                // Ignore animators that are a part of the body
                if (anim.GetComponentInParent<RigManager>()) continue;

                Timeline timeline = anim.GetComponent<Timeline>();
                if (timeline == null)
                    timeline = anim.gameObject.AddComponent<Timeline>();
                else
                    continue;
                timeline.mode = TimelineMode.Global;
                timeline._clock = clock;
                timeline.rewindable = true;
                clock.Register(timeline);
            }
            #endregion

            MelonCoroutines.Start(CoRun());

            // Force stop 4xSpeed if it's running
            foreach (var e in GlobalVariables.ActiveEffects) if (e.Name == "4x Speed") e.ForceEnd();
        }

        public override void OnEffectEnd()
        {
            clock.timeScale = 1;
            GlobalVariables.Player_BodyVitals.slowTimeEnabled = true;
            isRewindAlreadyActive = false;
        }
        
        private IEnumerator CoRun()
        {
            yield return new WaitForSecondsRealtime(15f);
            clock.timeScale = -1;

            Time.timeScale = 1;
        }
    }
}
