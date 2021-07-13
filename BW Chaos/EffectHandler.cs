using MelonLoader;
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using BW_Chaos.Effects;

namespace BW_Chaos
{
    internal class EffectHandler : MonoBehaviour
    {
        public EffectHandler(IntPtr ptr) : base(ptr) { }

        public static System.Collections.Generic.List<EffectBase> AllEffects;
        public static EffectHandler Instance;

        private int secondsEachEffect = 30;
        private int currentTimerValue;

        #region Wrist and Overlay Variables

        private Image overlayImage;
        private Text overlayText;
        private Image wristImage;
        private Text wristText;

        #endregion

        public void Start()
        {
            Instance = this;

            Canvas overlayCanvas = GameObject.Instantiate(GlobalVariables.OverlayChaosUI, transform).GetComponent<Canvas>();
            overlayImage = overlayCanvas.transform.Find("TimerImage").GetComponent<Image>();
            overlayImage.fillAmount = 0;
            overlayText = overlayCanvas.transform.Find("Text").GetComponent<Text>();

            Transform wristTransform = GlobalVariables.Player_RigManager.gameWorldSkeletonRig.characterAnimationManager.rightHandTransform;
            Canvas wristCanvas = GameObject.Instantiate(GlobalVariables.WristChaosUI, wristTransform).GetComponent<Canvas>();
            wristImage = wristCanvas.transform.Find("TimerImage").GetComponent<Image>();
            wristImage.fillAmount = 0;
            wristText = wristCanvas.transform.Find("Text").GetComponent<Text>();
            wristCanvas.transform.Reset();
            wristCanvas.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            wristCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            MelonCoroutines.Start(Timer());
        }

        public void Update()
        {
            // todo: find way to display "one-shot" effects (0 duration effects)
            string newString = string.Empty;
            foreach (EffectBase e in GlobalVariables.ActiveEffects)
                newString += e.Name + "\n";
            overlayText.text = newString;
            wristText.text = newString;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public IEnumerator Timer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                currentTimerValue += 1;
                float fillAmount = (float)currentTimerValue / secondsEachEffect;
                overlayImage.fillAmount = fillAmount;
                wristImage.fillAmount = fillAmount;

                if (currentTimerValue == secondsEachEffect)
                {
                    RunVotedEffect();
                    ResetEffectCandidates();
                }
            }
        }

        private void RunVotedEffect()
        {
            string messageData = GlobalVariables.WatsonClient.SendAndWaitAsync("sendvotes:").GetAwaiter().GetResult();
            int[] accumulatedVotes = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(messageData);  // todo: test this using the SendAndWait method

            MelonLogger.Msg(accumulatedVotes);

            (int, int) topVoted = (0, 0); // format is (arrIndex, value)
            for (int i = 0; i < accumulatedVotes.Length; i++)
            {
                if (accumulatedVotes[i] > topVoted.Item2)
                    topVoted = (i, accumulatedVotes[i]);
            }

            if (topVoted.Item1 == 4 || topVoted.Item2 == 0) // todo: for the second if condition here, we should choose a random one from the effect candidates
            {
                EffectBase e = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                MelonLogger.Msg(e.Name + " (random) was chosen");
                e.Run();
            }
            else
            {
                EffectBase e = GlobalVariables.CandidateEffects[topVoted.Item1];
                MelonLogger.Msg(e.Name + " was chosen");
                e.Run();
            }
        }

        private void ResetEffectCandidates()
        {
            // todo: add candidates text in overlay ui
            currentTimerValue = 0;
            try { overlayImage.fillAmount = 0; } catch { } // nullref sometimes happens here, no clue why but i added a trycatch for it
            try { wristImage.fillAmount = 0; } catch { }

            string botMesssage = "-- New Candidate Effects --";

            GlobalVariables.CandidateEffects.Clear();
            for (int i = 0; i < 4; i++)
            {
                EffectBase effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                while (GlobalVariables.CandidateEffects.Contains(effect))
                    effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                GlobalVariables.CandidateEffects.Add(effect);
                botMesssage += $"\n{i}: {effect.Name}";
            }

            GlobalVariables.WatsonClient.SendAsync("sendtochannel:" + botMesssage);
        }
    }
}
