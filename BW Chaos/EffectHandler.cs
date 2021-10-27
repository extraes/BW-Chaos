using BWChaos.Effects;
using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BWChaos
{
    [RegisterTypeInIl2Cpp]
    public class EffectHandler : MonoBehaviour
    {
        public EffectHandler(IntPtr ptr) : base(ptr) { }

        public static bool randomOnNoVotes = false;

        public static System.Collections.Generic.Dictionary<string, EffectBase> AllEffects = new System.Collections.Generic.Dictionary<string, EffectBase>();
        public static EffectHandler Instance;
        public static bool advanceTimer = false;

        private int secondsEachEffect = 30;
        private int currentTimerValue;
        private int numberFlip = 0;

        private IEnumerator timerInstance;

        #region Wrist and Overlay Variables

        private static Canvas overlayCanvas;
        private Image overlayImage;
        private Text overlayText;
        private Text candidateText;
        private Text voteText;
        private static Canvas wristCanvas;
        private Image wristImage;
        private Text wristText;

        private readonly Vector2 overlayImagePos_Candidates = new Vector2(-437.5f, -240);
        private readonly Vector2 overlayImagePos_NoCandidates = new Vector2(-300f, -240);

        private Image[] voteBars = new Image[5];

        #endregion

        public void Start()
        {
            Instance = this;
            // this is pretty messy looking, but in a nutshell
            // spawn overlaycanvas, move it to corner
            // spawn wristcanvas, adjust transform a bunch
            overlayCanvas = GameObject.Instantiate(GlobalVariables.OverlayChaosUI, transform).GetComponent<Canvas>();
            overlayImage = overlayCanvas.transform.Find("TimerImage").GetComponent<Image>();
            overlayImage.fillAmount = 0;
            if (Chaos.showCandidatesOnScreen) overlayImage.rectTransform.position = overlayImagePos_Candidates;

            overlayText = overlayCanvas.transform.Find("PastText").GetComponent<Text>();
            overlayText.text = string.Empty;
            candidateText = overlayCanvas.transform.Find("CandidateText").GetComponent<Text>();
            candidateText.horizontalOverflow = HorizontalWrapMode.Overflow;
            candidateText.text = string.Empty;
            var cEx = overlayCanvas.transform.Find("CandidateExtras");
            voteText = cEx.Find("VoteText").GetComponent<Text>();
            voteText.text = string.Empty;
            for (int i = 0; i < 5; i++)
                voteBars[i] = cEx.Find("VoteBar" + (i + 1)).GetComponent<Image>();

            Transform wristTransform = GlobalVariables.Player_RigManager.gameWorldSkeletonRig.characterAnimationManager.rightHandTransform;
            wristCanvas = GameObject.Instantiate(GlobalVariables.WristChaosUI, wristTransform).GetComponent<Canvas>();
            wristImage = wristCanvas.transform.Find("TimerImage").GetComponent<Image>();
            wristImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            wristImage.transform.localPosition = new Vector3(0, 20, 0);
            wristImage.fillAmount = 0;

            wristText = wristCanvas.transform.Find("Text").GetComponent<Text>();
            wristText.alignment = TextAnchor.LowerCenter;
            wristText.transform.Reset();
            wristText.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            wristText.transform.localPosition = new Vector3(0f, -1, 0f);
            // Fix blurry text by making font super fucking huge and shrinking the text on canvas lol
            wristText.fontSize = 72;
            wristText.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            wristText.horizontalOverflow = HorizontalWrapMode.Overflow;
            wristText.verticalOverflow = VerticalWrapMode.Overflow;

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
                string activeTag = e.Types.HasFlag(EffectBase.EffectTypes.HIDDEN) ? "HIDDEN" : (e.Duration - (int)(Time.realtimeSinceStartup - e.StartTime)).ToString();
                newString += e.Active ?
                    $"{e.Name} {activeTag}\n" :
                    $"{e.Name}\n";
            }

            overlayText.text = newString;
            // Hide all hidden effects, replace them with Immortality (because its relatively hard to discover)
            //todo: replace immortality with a randomized effect name
            wristText.text = Regex.Replace(newString, @".*HIDDEN", "Immortality", RegexOptions.Compiled | RegexOptions.ECMAScript);
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public IEnumerator Timer()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                if (!advanceTimer) continue;

                #region Set vote UI elements

                voteText.text = string.Empty;
                if (Chaos.showCandidatesOnScreen && Chaos.enableRemoteVoting)
                {
                    voteBars[0].transform.parent.gameObject.SetActiveRecursively(true);

                    int[] votes = GetVotes(); // to show effect votes live on the overlay
                    UpdateVoteBars(votes);
#if DEBUG
                    MelonLogger.Msg($"Recieved incomplete votes: [{string.Join(", ", votes)}]; Total: {votes.Sum()}");
#endif
                }
                else voteBars[0].transform.parent.gameObject.SetActiveRecursively(false);
                
                #endregion

                currentTimerValue += 1;
                float fillAmount = (float)currentTimerValue / secondsEachEffect;

                overlayImage.fillAmount = fillAmount;
                wristImage.fillAmount = fillAmount;

                if (currentTimerValue >= secondsEachEffect)
                {
                    RunVotedEffect();
                    ResetEffectCandidates();
                    voteText.text = string.Empty;
                }
            }
        }

        private IEnumerator SlerpVoteBar(Transform transform, Vector3 to)
        {
            yield return null;
            Vector3 from = transform.localScale;
            for (float i = 0; i < 1; i += 0.05f)
            {
                if (!transform) yield break; // null check because i dont want to do ondestroy shit
                transform.localScale = Vector3.Slerp(from, to, i);
                yield return new WaitForFixedUpdate();
            }
        }

        private void RunVotedEffect()
        {
            int[] accumulatedVotes = GetVotes();
            GlobalVariables.WatsonClient?.SendAsync("clearvotes:"); // Clear votes now that the time has come

            // Get the top voted effect
            (int, int) voted = GetVotedEffect(accumulatedVotes); // format is (arrIndex, value)
#if DEBUG
            MelonLogger.Msg($"Voted effect: {GlobalVariables.CandidateEffects[voted.Item1].Name}, [{string.Join(", ", accumulatedVotes)}]");
            if (voted.Item2 == 0) MelonLogger.Msg("The voted effect has no votes... Should I run a random effect? " + randomOnNoVotes);
            MelonLogger.Msg("Compound boolean statement of whether to return: " + (voted.Item2 == 0 && !randomOnNoVotes));
#endif
            // return if the top voted effect has no votes & modpref is set to not run on no votes
            if (voted.Item2 == 0 && !randomOnNoVotes) return;
            if (voted.Item1 == 4 || voted.Item2 == 0)
            {
                // Get a random effect from the dictionary
                AllEffects.Keys.ElementAt(UnityEngine.Random.Range(0, AllEffects.Keys.Count));
                EffectBase e = AllEffects[AllEffects.Keys.ElementAt(UnityEngine.Random.Range(0, AllEffects.Keys.Count))];
                MelonLogger.Msg(e.Name + " (random) was chosen");
                e.Run();
            }
            else
            {
                EffectBase e = GlobalVariables.CandidateEffects[voted.Item1];
                MelonLogger.Msg(e.Name + " was chosen");
                e.Run();
            }
            // Sometimes the timer/wristcanvas gets fucky with its offsets, so enforce them here
            wristImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            wristImage.transform.localPosition = new Vector3(0, 20, 0);
            wristText.transform.localPosition = new Vector3(0f, -1, 0);
        }

        private int[] GetVotes()
        {
            if (GlobalVariables.WatsonClient == null) return new int[] { 0, 0, 0, 0, 0 };
            string messageData = GlobalVariables.WatsonClient.SendAndWaitAsync("sendvotes:").GetAwaiter().GetResult();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(messageData);
        }

        private (int, int) GetVotedEffect(int[] votes)
        {
            if (Chaos.proportionalVoting)
            {
                // My jank-ass proportional voting system that works without floats, just ints.
                // heres a diagram in case you were confused https://discord.com/channels/563139253542846474/724595991675797554/898447182762479637 (lol)
                int votesTotal = 0;
                foreach (int vote in votes) votesTotal += vote;

                var ran = UnityEngine.Random.RandomRange(0, votesTotal) + 1;
                for (var index = 0; index < votes.Length; index++)
                {
                    if (ran - votes[index] <= 0)
                    {
                        return (index, votes[index]);
                    }
                    else ran -= votes[index];
                }
                return (0, 0);
            }
            else
            {
                // Traditional voting system, the one with the most votes wins.
                (int, int) topVoted = (0, 0);
                for (int i = 0; i < votes.Length; i++)
                {
                    if (votes[i] > topVoted.Item2)
                        topVoted = (i, votes[i]);
                }
                return topVoted;
            }
        }

        private void UpdateVoteBars(int[] votes)
        {
            float total = votes[0] + votes[1] + votes[2] + votes[3] + votes[4]; // sum immediately as float for float division for votebarproportion

            for (int i = 0; i < 5; i++)
            {
                float voteBarProportion = total == 0 ? 0.5f : (votes[i] / total); // Avoid divide by zero error
                MelonCoroutines.Start(SlerpVoteBar(voteBars[i].transform, new Vector3(0.5f + (voteBarProportion / 2), 1, 0))); // scale the bars proportionaltely, depending on what percentage of votes they have.
                voteText.text += votes[i] + "\n";
            }
            voteText.text += "Total: " + total;
        }

        private void ResetEffectCandidates()
        {
            // reset GUI values to default
            currentTimerValue = 0;
            overlayImage.fillAmount = 0;
            wristImage.fillAmount = 0;
            UpdateVoteBars(new int[] { 0, 0, 0, 0, 0 });

            string botMesssage = "-- New Candidate Effects --";

            GlobalVariables.CandidateEffects.Clear();

            // Flip numbers to avoid confusion due to latency.
            if (numberFlip == 0) numberFlip = 5;
            else numberFlip = 0;
            GlobalVariables.WatsonClient?.SendAsync("flipnumbers:" + numberFlip); // is this wasteful? probably. does it ensure the two stay in sync? probably.

            for (int i = 0; i < 4; i++)
            {
                // Get a random effect from the list
                EffectBase effect = AllEffects[AllEffects.Keys.ElementAt(UnityEngine.Random.Range(0, AllEffects.Keys.Count))];

                // Make sure the effect is unique in the list
                while (GlobalVariables.CandidateEffects.Contains(effect))
                    effect = AllEffects[AllEffects.Keys.ElementAt(UnityEngine.Random.Range(0, AllEffects.Keys.Count))];
                GlobalVariables.CandidateEffects.Add(effect);

                botMesssage += $"\n{numberFlip + i + 1}: {effect.Name}";
                // For twitch, enable number flipping
            }

            botMesssage += $"\n{5 + numberFlip}: Random Effect";

            if (Chaos.sendCandidatesInChannel && advanceTimer) GlobalVariables.WatsonClient?.SendAsync("sendtochannel:" + botMesssage);

            if (Chaos.showCandidatesOnScreen && advanceTimer)
            {
                candidateText.text = botMesssage.Replace("-- New Candidate Effects --\n", ""); // Skip the first line of botmessage
                overlayImage.rectTransform.anchoredPosition = overlayImagePos_Candidates;
            }
            else
            {
                candidateText.text = string.Empty;
                if (!advanceTimer) overlayImage.rectTransform.anchoredPosition = overlayImagePos_NoCandidates;
            }
        }
    }
}
