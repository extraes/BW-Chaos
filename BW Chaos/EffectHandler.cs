using BWChaos.Effects;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public static Dictionary<string, EffectBase> AllEffects = new Dictionary<string, EffectBase>();
        public static EffectHandler Instance;
        public static bool advanceTimer = false;

        private int secondsEachEffect = 30;
        private int currentTimerValue;
        private int numberFlip = 0;

        private object timerToken;

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
            if (Prefs.ShowCandidatesOnScreen) overlayImage.rectTransform.position = overlayImagePos_Candidates;

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

            // Start the timer immediately
            timerToken = MelonCoroutines.Start(Timer());
            ResetEffectCandidates();
        }

        public void OnDestroy()
        {
            MelonCoroutines.Stop(timerToken);
            // foreach throws "System.InvalidOperationException: Collection was modified; enumeration operation may not execute."
            // maybe this will fix?
            
            for (int i = 0;  GlobalVariables.ActiveEffects.Count != 0; i++)
            {
#if DEBUG
                Chaos.Log($"Iteration: {i}, Ending effect {GlobalVariables.ActiveEffects[0]?.Name ?? "null (wha?)"}");
#endif
                GlobalVariables.ActiveEffects[0].ForceEnd();
                if (i < 100) break; // if forceend didnt do shit to an effect, just forget it
            }

            GlobalVariables.ActiveEffects.Clear();
            GlobalVariables.CandidateEffects.Clear();
            GlobalVariables.PreviousEffects.Clear();
        }

        public void Update()
        {
            string newString = GlobalVariables.PreviousEffects.Join("\n") + "\n";
            //newString += GlobalVariables.PreviousEffects.Join("\n");
            // Hide hidden effects (Like FakeCrash) from the player
            List<string> activeTags = new List<string>();
            foreach (EffectBase e in GlobalVariables.ActiveEffects)
            {
                activeTags.Clear();
                activeTags.Add((e.Duration - (int)(Time.realtimeSinceStartup - e.StartTime)).ToString());
                if (e.Types.HasFlag(EffectBase.EffectTypes.HIDDEN)) activeTags.Add("HIDDEN");
                if (e.Types.HasFlag(EffectBase.EffectTypes.META)) activeTags.Add("META");

                newString += e.Active ?
                    $"{e.Name} {activeTags.Join(" ")}\n" :
                    $"{e.Name}\n";
            }


            overlayText.text = newString;
            // Hide all hidden effects, replace them with Immortality (because its relatively hard to discover)
            //todo: replace immortality with a randomized effect name
            wristText.text = Regex.Replace(newString, @".*HIDDEN", "Immortality", RegexOptions.Compiled | RegexOptions.ECMAScript);
        }

        public void OnTriggerEnter(Collider col)
        {
            // The load zone triggers are named TRIGGER_EXIT; If this fails, change to the Fizzlers instead.
            if (col.name == "TRIGGER_EXIT")
            {
#if DEBUG
                Chaos.Log("Caught Trigger Exit before this gameobject was actually destroyed, cool!");
#endif
                GameObject.Destroy(this);
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public IEnumerator Timer()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                // Timescale being 0 usually means the player is in the steamvr menu, we dont want anything happening then.
                if (!advanceTimer || Time.timeScale == 0 || Time.deltaTime > 0.1f) continue;

                #region Set vote UI elements

                voteText.text = string.Empty;
                if (Prefs.ShowCandidatesOnScreen && Prefs.EnableRemoteVoting)
                {
                    voteBars[0].transform.parent.gameObject.SetActiveRecursively(true);

                    int[] votes = GetVotes(); // to show effect votes live on the overlay
                    UpdateVoteBars(votes);
#if DEBUG
                    Chaos.Log($"Recieved incomplete votes: [{string.Join(", ", votes)}]; Total: {votes.Sum()}");
#endif
                }
                else voteBars[0].transform.parent.gameObject.SetActiveRecursively(false);

                #endregion

                wristCanvas.gameObject.SetActive(Prefs.ShowWristUI);

                currentTimerValue += 1;
                float fillAmount = (float)currentTimerValue / secondsEachEffect;

                MelonCoroutines.Start(SlerpUICirle(overlayImage, fillAmount));
                MelonCoroutines.Start(SlerpUICirle(wristImage, fillAmount));

                if (currentTimerValue >= secondsEachEffect)
                {
                    RunVotedEffect();
                    ResetEffectCandidates();
                    voteText.text = string.Empty;
                }
            }
        }

        // Slerp the vote bars according to how many votes each one has, an animation makes it look pretty!
        private IEnumerator SlerpVoteBar(Transform transform, Vector3 to)
        {
            yield return null;
            Vector3? from = transform?.localScale;
            for (float i = 0; i < 1; i += 0.05f)
            {
                if (transform == null) yield break; // null check because i dont want to do ondestroy shit
                transform.localScale = Vector3.Slerp((Vector3)from, to, i);
                yield return new WaitForFixedUpdate();
            }
        }

        // Slerp GUI for the same reason, prettiness!
        private IEnumerator SlerpUICirle(Image img, float to)
        {
            if (to == 1) to = 0;
            (float, float) tup = (img.fillAmount, to);
            yield return null;
            for (float i = 0; i < 1; i += 0.05f)
            {
                if (img == null) yield break; // null check because i dont want to do ondestroy shit
                img.fillAmount = tup.Slerp(i);
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
            Chaos.Log($"Voted effect: {(voted.Item1 == 4 ? "Random" : GlobalVariables.CandidateEffects[voted.Item1].Name)} (of type '{(voted.Item1 == 4 ? "Random" : GlobalVariables.CandidateEffects[voted.Item1].GetType().Name)}'), [{string.Join(", ", accumulatedVotes)}]");
            if (voted.Item2 == 0) Chaos.Log("The voted effect has no votes... Should I run a random effect? " + randomOnNoVotes);
            Chaos.Log("Compound boolean statement of whether to return: " + (voted.Item2 == 0 && !randomOnNoVotes));
#endif
            Type votedType;
            // return if the top voted effect has no votes & modpref is set to not run on no votes
            if (voted.Item2 == 0 && !randomOnNoVotes) return;
            if (voted.Item1 == 4 || voted.Item2 == 0)
            {
                // Get a random effect from the dictionary
                EffectBase e = AllEffects.Random().Value;
                Chaos.Log(e.Name + " (random) was chosen");
                votedType = e.GetType();
            }
            else
            {
                EffectBase e = GlobalVariables.CandidateEffects[voted.Item1];
                Chaos.Log(e.Name + " was chosen");
                votedType = e.GetType();
            }
            EffectBase eff = (EffectBase)Activator.CreateInstance(votedType);
            eff.Run();

            // Sometimes the timer/wristcanvas gets fucky with its offsets, so reinforce them here
            wristImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            wristImage.transform.localPosition = new Vector3(0, 20, 0);
            wristText.transform.localPosition = new Vector3(0f, -1, 0);
        }

        private int[] GetVotes()
        {
            // Perform a null check to make sure 
            if (GlobalVariables.WatsonClient == null) return new int[] { 0, 0, 0, 0, 0 };
            string messageData = GlobalVariables.WatsonClient.SendAndWaitAsync("sendvotes:").GetAwaiter().GetResult();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(messageData);
        }

        private (int, int) GetVotedEffect(int[] votes)
        {
            if (Prefs.ProportionalVoting)
            {
                // My gigabrain proportional voting system that works without floats, just ints.
                // heres a diagram in case you were confused https://discord.com/channels/563139253542846474/724595991675797554/898447182762479637 (lol)
                // Check out ChaosModV's version, apparently i had the same idea as they did, but i did mine by adding instead of subtracting or something
                
                // First order of business: sum the votes
                int votesTotal = votes.Sum();

                // Get a random number between 0 and the total, inclusive
                var ran = UnityEngine.Random.RandomRange(0, votesTotal) + 1;
                for (var index = 0; index < votes.Length; index++)
                {
                    // If the remainder of ran is within this index's vote count, that means this effect wins
                    if (ran - votes[index] <= 0)
                    {
                        return (index, votes[index]);
                    }
                    // Otherwise, subtract it and keep going
                    else ran -= votes[index];
                }
                // Fallback in case there were no votes or my code fucked up.
                return (0, 0);
            }
            else
            {
                // Traditional voting system, the one with the most votes wins.
                (int, int) topVoted = (0, 0);
                for (int i = 0; i < votes.Length; i++)
                {
                    // Standard "if this vote is bigger than what we already have, set the voted to it instead" approach
                    if (votes[i] > topVoted.Item2)
                        topVoted = (i, votes[i]);
                }
                return topVoted;
            }
        }

        private void UpdateVoteBars(int[] votes)
        {
            float total = votes.Sum(); // sum immediately as float for float division for votebarproportion

            for (int i = 0; i < votes.Length; i++)
            {
                float voteBarProportion = total == 0 ? 0.5f : (votes[i] / total); // Avoid divide by zero error
                // scale the bars proportionaltely, depending on what percentage of votes they have, but keep it from going to zero, cause that doesn't look as cool lol
                MelonCoroutines.Start(SlerpVoteBar(voteBars[i].transform, new Vector3(0.5f + (voteBarProportion / 2), 1, 0)));
                voteText.text += votes[i] + "\n";
            }
            voteText.text += "Total: " + total;
        }

        private void ResetEffectCandidates()
        {
            // reset GUI values to default
            currentTimerValue = 0;
            //MelonCoroutines.Start(SlerpUICirle(overlayImage, 0));     commented cause it would interfere with existing coroutines,
            //MelonCoroutines.Start(SlerpUICirle(wristImage, 0));       and cause i just added a check in there

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

                // Add 1 because humans don't like zero based indices (FUCK LUA)
                botMesssage += $"\n{numberFlip + i + 1}: {effect.Name}";
            }

            botMesssage += $"\n{5 + numberFlip}: Random Effect";

            if (Prefs.SendCandidatesInChannel && advanceTimer) GlobalVariables.WatsonClient?.SendAsync("sendtochannel:" + botMesssage);

            if (Prefs.ShowCandidatesOnScreen && advanceTimer)
            {
                candidateText.text = string.Join("\n", botMesssage.Split('\n').Skip(1)); // Skip the first line of botmessage
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
