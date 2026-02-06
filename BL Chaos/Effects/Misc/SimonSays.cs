#if !NOBONELIB
using Il2CppSLZ.Interaction;
using Il2CppSLZ.Marrow.PuppetMasta;
using Jevil.Patching;

namespace BLChaos.Effects;

// Wow holy fuck I am so sorry for writing code this bad.
// This is like, really fucking bad.
// I tried to make it not bad. Clearly I have failed.
internal class SimonSays : EffectBase
{
    public SimonSays() : base("Simon Says", 60, EffectTypes.DONT_SYNC)
    {
        DieDelegate = _ => Die();
    }
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
    [RangePreference(5, 30, 1)] static int roundTime = 15;
    private static Il2CppSystem.Action<PuppetMaster> DispatchNpcDeath = new Action<PuppetMaster>(_ => killNpc?.Invoke());
    private static Action? killNpc;
    private static Action? buttonPress;
    private Action<RigManager> DieDelegate;

    public override void OnEffectStart()
    {
        ResetConditions();

        PuppetMaster.add_OnDeathStatsEvent(DispatchNpcDeath);

        GameCallbacks.OnJump += Jump;
        GameCallbacks.OnPuppetMasterDeath += Kill;
        GameCallbacks.OnButtonPress += ButtonPress;
        Hooking.OnPlayerDeath += DieDelegate;
    }

    public override void OnEffectEnd()
    {
        Utilities.Try(() => PuppetMaster.remove_OnDeathStatsEvent(DispatchNpcDeath));

        GameCallbacks.OnJump -= Jump;
        GameCallbacks.OnPuppetMasterDeath -= Kill;
        GameCallbacks.OnButtonPress -= ButtonPress;
        Hooking.OnPlayerDeath -= DieDelegate;
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
            string mName = Utilities.GenerateFriendlyMemberName(pair.Value);
            bool didSimonSay = Random.value > 0.5f;
            Vector3 startPos = GlobalVariables.Player_PhysRig.torso.transform.position;

            //NotificationData nDat = Notifications.SendNotification(, 5);

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
                    conditions[sst] = Player.GetComponentInHand<Gun>(Player.LeftHand) || Player.GetComponentInHand<Gun>(Player.RightHand);
                    break;
                case SimonSaysType.duck:
                    Vector3 posFeet = GlobalVariables.Player_PhysRig.rbFeet.transform.position;
                    Vector3 posHead = GlobalVariables.Player_PhysRig.torso.rbHead.transform.position;
                    float dist = Vector3.Distance(posFeet, posHead);
                    conditions[sst] = dist < 0.5 * GlobalVariables.Player_RigManager.avatar.height;
                    
#if DEBUG
                    Log("Distance between head and feet: " + dist);
#endif
                    break;
                case SimonSaysType.dropYourItems:
                    conditions[sst] = !(Player.GetObjectInHand(Player.LeftHand) || Player.GetObjectInHand(Player.RightHand));
                    break;
                case SimonSaysType.dontMove:
                    conditions[sst] = Vector3.Distance(startPos, GlobalVariables.Player_PhysRig.transform.position) < 1;
                    break;
                case SimonSaysType.holdItemsInBothHands:
                    conditions[sst] = !(Player.GetObjectInHand(Player.LeftHand) == null || Player.GetObjectInHand(Player.RightHand) == null);
                    break;
                default:
                    break;
            }

            bool doPunish = didSimonSay != conditions[sst];
#if DEBUG
            Log($"Did the player {sst}? {conditions[sst]}. Punish? {doPunish}");
#endif
            if (doPunish) GlobalVariables.Player_Health.Death();
        }
    }

    private void Jump() => conditions[SimonSaysType.jump] = true;
    private void Kill(PuppetMaster _) => conditions[SimonSaysType.killAnNpc] = true;
    private void Die() => conditions[SimonSaysType.die] = true;
    private void ButtonPress() => conditions[SimonSaysType.pushAButton] = true;

}
#endif
