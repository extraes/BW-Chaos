using MelonLoader;
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using BW_Chaos.Effects;
using System.Threading.Tasks;
using System.Linq;

namespace BW_Chaos
{
    internal class EffectHandler : MonoBehaviour
    {
        public EffectHandler(IntPtr ptr) : base(ptr) { }

        public static System.Collections.Generic.List<EffectBase> AllEffects;
        public static EffectHandler Instance;

        public int secondsEachEffect = 30;
        public int currentTimerValue;
        public Text text;

        private Image mask;

        private float timeSinceEnabled = 0f;
        private bool showGUI = true;

        public void Start()
        {
            Instance = this;

            mask = GetComponent<Image>();
            MelonCoroutines.Start(Timer());
        }

        public void Update()
        {
            string newString = string.Empty;
            foreach (EffectBase e in GlobalVariables.ActiveEffects)
                newString += e.Name + "\n";
            text.text = newString;
        }

        /*public void OnGUI()
        {
            if (showGUI)
            {
                *//*float timeSinceReset = Time.timeSinceLevelLoad - timeSinceEnabled;
                if (!(timeSinceReset >= 30))
                {
                    GUI.Box(new Rect(50, 25, 350, 25),
                        "BW Chaos: Waiting " + (30 - Math.Floor(timeSinceReset)) + " seconds before starting");
                    GUI.Box(new Rect(50, 50, 350, 25), "Made by extraes");
                    return;
                }*//*

                int effectNumber = 0;
                foreach (EffectBase effect in GlobalVariables.CandidateEffects)
                {
                    GUI.Box(new Rect(50, 50 + (effectNumber * 25), 500, 25), $"{effectNumber + 1}: {effect.Name}");
                    effectNumber++;
                }
                GUI.Box(new Rect(50, 50 + (effectNumber * 25), 500, 25), $"{effectNumber + 1}: Random Effect");
                GUI.Box(new Rect(50, 250, 500, (GlobalVariables.ActiveEffects.Count + 1) * 20 + 10), "Active effects:\n" + string.Join("\n", GlobalVariables.ActiveEffects));
                GUI.Box(new Rect(Screen.width - 550, 50, 500, 25), "Time");
                GUI.Box(new Rect(Screen.width - 550, 75, 500 * Math.Min(timeSinceReset % 30 / 30, 1f), 25), "");
            }
        }*/

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public IEnumerator Timer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                currentTimerValue += 1;
                mask.fillAmount = (float)currentTimerValue / secondsEachEffect;

                if (currentTimerValue == secondsEachEffect)
                {
                    RunVotedEffect();
                    ResetEffectCandidates();
                }
            }
        }

        private async void RunVotedEffect()
        {
            await GlobalVariables.WatsonClient.SendAsync("sendvotes:");
            while (GlobalVariables.AccumulatedVotes == null) await Task.Delay(250);

            MelonLogger.Msg(GlobalVariables.AccumulatedVotes);

            (int, int) topVoted = (0, 0); // format is (arrIndex, value)
            for (int i = 0; i < GlobalVariables.AccumulatedVotes.Length; i++)
            {
                if (GlobalVariables.AccumulatedVotes[i] > topVoted.Item2)
                    topVoted = (i, GlobalVariables.AccumulatedVotes[i]);
            }

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
            currentTimerValue = 0;
            lerp = 0;
            try { mask.fillAmount = 0; } catch { }

            GlobalVariables.CandidateEffects.Clear();
            for (int i = 0; i < 4; i++)
            {
                EffectBase effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                while (GlobalVariables.CandidateEffects.Contains(effect))
                    effect = AllEffects[UnityEngine.Random.Range(0, AllEffects.Count)];
                GlobalVariables.CandidateEffects.Add(effect);
            }
        }

        private void LogEffect(string effectName)
        {
            // TODO: Handle lengthed effects
            if (text == null)
                return;

            text.text += $"\n{effectName}";
            string[] lineArray = text.text.Split(Environment.NewLine.ToCharArray());

            if (lineArray.Length > 20)
                text.text.RemoveFirstLines(1);
        }
    }
}
