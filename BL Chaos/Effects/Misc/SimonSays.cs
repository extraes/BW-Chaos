using HarmonyLib;
using ModThatIsNotMod;
using PuppetMasta;
using StressLevelZero.Interaction;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

// Wow holy fuck I am so sorry for writing code this bad.
// This is like, really fucking bad.
// I tried to make it not bad. Clearly I have failed.
internal class SimonSays : EffectBase
{
    public SimonSays() : base("Simon Says", 60, EffectTypes.DONT_SYNC) { }
    private enum SimonSaysType
    {
        die,
        grabAGun,
        killAnNpc,
        jump,
        duck,
        pushAButton,
        dropYourItems,
        dontMove,
        holdItemsInBothHands,
    }

    readonly Dictionary<SimonSaysType, bool> conditions = new Dictionary<SimonSaysType, bool>();
    [RangePreference(5, 30, 1)] static readonly int roundTime = 15;
    private static Action killNpc;
    private static Action jump;
    private static Action buttonPress;

    public override void OnEffectStart()
    {
        ResetConditions();

        jump += Jump;
        killNpc += Kill;
        buttonPress += ButtonPress;
        Hooking.OnPlayerDeath += Die;

        GameObject.FindObjectsOfType<ButtonToggle>().ForEach(b => b.onPress.AddListener(buttonPress));
    }

    public override void OnEffectEnd()
    {
        jump -= Jump;
        killNpc -= Kill;
        buttonPress -= ButtonPress;
        Hooking.OnPlayerDeath -= Die;

        GameObject.FindObjectsOfType<ButtonToggle>().ForEach(b => b.onPress.RemoveListener(buttonPress));
    }

    private void ResetConditions()
    {
        conditions.Clear();
        foreach (object val in Enum.GetValues(typeof(SimonSaysType)))
        {
            conditions.Add((SimonSaysType)val, false);
        }
    }


    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        yield return null;
        SimonSaysType[] vals = (SimonSaysType[])Enum.GetValues(typeof(SimonSaysType));
        string[] names = Enum.GetNames(typeof(SimonSaysType));
        //like .NET Core's .Zip linq extension, but manual
        int i = 0;
        KeyValuePair<SimonSaysType, string>[] kv = names.Select(n => new KeyValuePair<SimonSaysType, string>(vals[i++], n)).ToArray();

        while (Active)
        {
            KeyValuePair<SimonSaysType, string> pair = kv.Random();
            SimonSaysType sst = pair.Key;
            string mName = Utilities.GetReadableStringFromMemberName(pair.Value);
            bool didSimonSay = Random.value > 0.5f;
            Vector3 startPos = GlobalVariables.Player_PhysBody.transform.position;

            NotificationData nDat = Notifications.SendNotification((didSimonSay ? "Simon says " : "") + mName, 5);
            yield return new WaitForSecondsRealtime(roundTime);

            // cond = true; dss = true; do not punish
            // cond = false; dss = false; do not punish
            // anything else, do punish. 
            // sounds good to me

            switch (sst)
            {
                case SimonSaysType.die:
                case SimonSaysType.killAnNpc:
                case SimonSaysType.jump:
                case SimonSaysType.pushAButton:
                    // These cases are already handled by patches/hooks elsewhere
                    break;
                case SimonSaysType.grabAGun:
                    conditions[sst] = Player.GetGunInHand(Player.leftHand) || Player.GetGunInHand(Player.rightHand);
                    break;
                case SimonSaysType.duck:
                    Vector3 posFeet = GlobalVariables.Player_PhysBody.rbFeet.transform.position;
                    Vector3 posHead = GlobalVariables.Player_PhysBody.rbHead.transform.position;
                    float dist = Vector3.Distance(posFeet, posHead);
                    conditions[sst] = dist < 1;
#if DEBUG
                    Log("Distance between head and feet: " + dist);
#endif
                    break;
                case SimonSaysType.dropYourItems:
                    conditions[sst] = !(Player.GetObjectInHand(Player.leftHand) || Player.GetObjectInHand(Player.rightHand));
                    break;
                case SimonSaysType.dontMove:
                    conditions[sst] = Vector3.Distance(startPos, GlobalVariables.Player_PhysBody.transform.position) < 1;
                    break;
                case SimonSaysType.holdItemsInBothHands:
                    conditions[sst] = !(Player.GetObjectInHand(Player.leftHand) == null || Player.GetObjectInHand(Player.rightHand) == null);
                    break;
                default:
                    break;
            }

            bool doPunish = didSimonSay != conditions[sst];
#if DEBUG
            Log($"Did the player {sst}? {conditions[sst]}. Punish? {doPunish}");
#endif
            if (doPunish) GlobalVariables.Player_Health?.Death();
        }
    }

    private void Jump() => conditions[SimonSaysType.jump] = true;
    private void Kill() => conditions[SimonSaysType.killAnNpc] = true;
    private void Die() => conditions[SimonSaysType.die] = true;
    private void ButtonPress() => conditions[SimonSaysType.pushAButton] = true;

    [HarmonyPatch(typeof(BehaviourBaseNav), nameof(BehaviourBaseNav.KillStart))]
    public static class KillPatch
    {
        public static void Postfix()
        {
            killNpc?.Invoke();
        }
    }

    [HarmonyPatch(typeof(ButtonToggle), nameof(ButtonToggle.Awake))]
    public static class ButtonPatch
    {
        public static void Postfix(ButtonToggle __instance)
        {
            __instance.onPress.AddListener(buttonPress);
        }
    }

    [HarmonyPatch(typeof(ControllerRig), "Jump")]
    public class ControllerRigJumpPatch
    {
        public static void Postfix()
        {
            PhysGrounder physGrounder = GameObject.FindObjectOfType<PhysGrounder>();

            // Only jump when on the ground
            if (physGrounder.isGrounded)
            {
                jump?.Invoke();
            }
        }
    }
}
