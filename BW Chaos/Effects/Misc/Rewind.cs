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
        public override void OnEffectStart()
        {
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

        }

        public override void OnEffectUpdate()
        {
            clock.timeScale = 1;
        }
        
        private IEnumerator CoRun()
        {
            yield return new WaitForSecondsRealtime(15f);
            clock.timeScale = -1;

            // To foolproof this effect, override Time.timeScale (By setting it every frame :DDDDD)
            var startRewindTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < startRewindTime + 15)
            {
                Time.timeScale = 1;
                yield return new WaitForFixedUpdate();
            }

        }
    }
}
