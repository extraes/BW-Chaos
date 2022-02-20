using Chronos;
using MelonLoader;
using StressLevelZero.Rig;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class Rewind : EffectBase
    {
        public Rewind() : base("Record then rewind 15 seconds", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
        private static GlobalClock clock = null;
        private static bool isRewindAlreadyActive = false; // We don't want two rewinds at the same time. That would be bad.
        public override void OnEffectStart()
        {
            if (isRewindAlreadyActive)
            {
                Chaos.Warn("Two instances of Rewind can't be active at the same time! Not running again!");
            }

            isRewindAlreadyActive = true;
            Utilities.DisableSloMo();

            #region Set up Chronos & clock

            GameObject chronos = GameObject.Find("ChronosController") ?? new GameObject("ChronosController");
            if (clock == null) clock = chronos.AddComponent<GlobalClock>();
            
            clock.key = "Root";
            //                  V can't do ?., so we try catch.
            try { if (Timekeeper.instance == null) chronos.AddComponent<Timekeeper>(); }
#if !DEBUG
            catch
            {
#else
            catch (System.Exception err)
            {
                Chaos.Warn("Caught Singleton exception, adding Timekeeper now. In case you wanted the error, here:");
                Chaos.Warn(err);
#endif
                chronos.AddComponent<Timekeeper>();
            }

            #endregion

            #region Register Rb's into clock

            foreach (Rigidbody rb in Utilities.FindAll<Rigidbody>())
            {
                if (rb.GetComponentInParent<RigManager>() /*|| rb.IsSleeping()*/ || rb.isKinematic) continue;

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
                timeline._recordingDuration = 15f;
                timeline.recordingInterval = 0.25f;
                timeline._recordingInterval = 0.25f;
                clock.Register(timeline);
            }

            #endregion

            #region Register animators into clock

            foreach (Animator anim in Utilities.FindAll<Animator>())
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
            while (GlobalVariables.ActiveEffects.Any(e => e.Name == "4x speed")) GlobalVariables.ActiveEffects.FirstOrDefault(e => e.Name == "4x speed")?.ForceEnd();
        }

        public override void OnEffectUpdate() => Time.timeScale = 1;

        public override void OnEffectEnd()
        {
            clock.timeScale = 1;
            Utilities.EnableSloMo();
            isRewindAlreadyActive = false;
            foreach (var tl in Utilities.FindAll<Timeline>()) // hopefully that works?
            {
                var rb = tl.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    // prevents objects from jumping, but freezes the fuck out of NPC's. this is janky but prevents npc freeze
                    if (rb.GetComponent<StressLevelZero.AI.AIBrain>() == null) rb.Sleep(); 
                }
                
                GameObject.Destroy(tl);
            }
        }
        
        private IEnumerator CoRun()
        {
            yield return new WaitForSecondsRealtime(14f);
            clock.LerpTimeScale(-1, 1);
            yield return new WaitForSecondsRealtime(15f);
            clock.LerpTimeScale(1, 1);
        }
    }
}
