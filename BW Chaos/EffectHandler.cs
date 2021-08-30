using MelonLoader;
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using BWChaos.Effects;
using System.Text.RegularExpressions;

namespace BWChaos
{
    [RegisterTypeInIl2Cpp]
    internal class EffectHandler : MonoBehaviour
    {
        public EffectHandler(IntPtr ptr) : base(ptr) { }

        private bool randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");

        public static System.Collections.Generic.List<EffectBase> AllEffects;
        public static EffectHandler Instance;

        private int secondsEachEffect = 30;
        private int currentTimerValue;

        private IEnumerator timerInstance;

        #region Wrist and Overlay Variables

        private Image overlayImage;
        private Text overlayText;
        private Image wristImage;
        private Text wristText;

        #endregion

        public void Start()
        {
            Instance = this;

            // this is pretty messy looking, but in a nutshell
            // spawn overlaycanvas, move it to corner
            // spawn wristcanvas, adjust transform a bunch
            Canvas overlayCanvas = GameObject.Instantiate(GlobalVariables.OverlayChaosUI, transform).GetComponent<Canvas>();
            overlayImage = overlayCanvas.transform.Find("TimerImage").GetComponent<Image>();
            overlayImage.fillAmount = 0;
            overlayImage.GetComponent<RectTransform>().position =
                new Vector3(Screen.width - 200, Screen.height - 201, 0);
            overlayText = overlayCanvas.transform.Find("Text").GetComponent<Text>();
            overlayText.alignment = TextAnchor.LowerCenter;
            overlayText.GetComponent<RectTransform>().position
                = new Vector3(Screen.width - 200, Screen.height - 80, 0);

            Transform wristTransform = GlobalVariables.Player_RigManager.gameWorldSkeletonRig.characterAnimationManager.rightHandTransform;
            Canvas wristCanvas = GameObject.Instantiate(GlobalVariables.WristChaosUI, wristTransform).GetComponent<Canvas>();
            wristImage = wristCanvas.transform.Find("TimerImage").GetComponent<Image>();
            wristImage.fillAmount = 0;
            wristText = wristCanvas.transform.Find("Text").GetComponent<Text>();
            wristText.alignment = TextAnchor.LowerCenter;
            wristText.transform.Reset();
            wristText.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            wristText.transform.localPosition = new Vector3(0f, -0.25f, 0f);
            wristCanvas.transform.Reset();
            wristCanvas.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            wristCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            timerInstance = (IEnumerator)MelonCoroutines.Start(Timer());
            ResetEffectCandidates();
        }

        public void OnDestroy()
        {
            MelonCoroutines.Stop(timerInstance);
            foreach (EffectBase e in GlobalVariables.ActiveEffects) e.ForceEnd();

            GlobalVariables.ActiveEffects.Clear();
            GlobalVariables.CandidateEffects.Clear();
            GlobalVariables.PreviousEffects.Clear();
        }

        public void Update()
        {
            string newString = string.Empty;
            // Hide hidden effects (Like FakeCrash) from the player
            foreach (EffectBase e in GlobalVariables.PreviousEffects)
            {
                // Is this more readable than ternary operatoring in a ternary operator?
                var activeTag = e.Types.HasFlag(EffectBase.EffectTypes.HIDDEN) ? "HIDDEN" : "ACTIVE";
                newString += e.Active ?
                    $"{e.Name} {activeTag}\n": 
                    $"{e.Name}\n";
            }

            overlayText.text = newString;
            // Hide all hidden effects, replace them with Immortality (because its relatively hard to discover)
            //todo: replace immortality with a randomized effect name
            wristText.text = Regex.Replace(newString, @".*\(HIDDEN\)", "Immortality", RegexOptions.Compiled | RegexOptions.ECMAScript);
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public IEnumerator Timer()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);

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
            int[] accumulatedVotes = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(messageData);

            MelonLogger.Msg(messageData);

            (int, int) topVoted = (0, 0); // format is (arrIndex, value)
            for (int i = 0; i < accumulatedVotes.Length; i++)
            {
                if (accumulatedVotes[i] > topVoted.Item2)
                    topVoted = (i, accumulatedVotes[i]);
            }
            
            if (topVoted.Item2 == 0 && !randomOnNoVotes) return; // todo: find a better, not shit way to do this - extraes
            if (topVoted.Item1 == 4 || topVoted.Item2 == 0)
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
            overlayImage.fillAmount = 0;
            wristImage.fillAmount = 0;

            string botMesssage = "-- New Candidate Effects --";

            GlobalVariables.CandidateEffects.Clear();
            for (int i = 0; i < 4; i++)
            {
                EffectBase effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                while (GlobalVariables.CandidateEffects.Contains(effect))
                    effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                GlobalVariables.CandidateEffects.Add(effect);
                botMesssage += $"\n{i + 1}: {effect.Name}";
                // do we really need to do any number flipping? this aint twitch chat, you get slow mode anyway
            }

            botMesssage += "\n5: Random Effect";
            GlobalVariables.WatsonClient.SendAsync("sendtochannel:" + botMesssage);
        }
    }
}
