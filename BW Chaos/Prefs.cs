using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using static BWChaos.Effects.EffectBase;

namespace BWChaos;

public static class Prefs
{
    internal const string CATEGORY_NAME = "BW_Chaos";
    private static MelonPreferences_Category category;
    internal static MelonPreferences_Entry<bool> randomOnNoVotes;
    internal static MelonPreferences_Entry<bool> useLaggyEffects;
    internal static MelonPreferences_Entry<bool> useGravityEffects;
    internal static MelonPreferences_Entry<bool> useSteamProfileEffects;
    internal static MelonPreferences_Entry<bool> useMetaEffects;
    internal static MelonPreferences_Entry<bool> showCandidatesOnScreen;
    internal static MelonPreferences_Entry<bool> scrollCandidates;
    internal static MelonPreferences_Entry<bool> showWristUI;
    internal static MelonPreferences_Entry<bool> sendCandidatesInChannel;
    internal static MelonPreferences_Entry<bool> ignoreRepeatVotes;
    internal static MelonPreferences_Entry<bool> proportionalVoting;
    internal static MelonPreferences_Entry<bool> enableRemoteVoting;
    internal static MelonPreferences_Entry<bool> syncEffects;
    internal static MelonPreferences_Entry<bool> useBagRandomizer;

    internal static bool RandomOnNoVotes => randomOnNoVotes.Value;
    internal static bool UseLaggyEffects => useLaggyEffects.Value;
    internal static bool UseGravityEffects => useGravityEffects.Value;
    internal static bool UseSteamProfileEffects => useSteamProfileEffects.Value;
    internal static bool UseMetaEffects => useMetaEffects.Value;
    internal static bool ShowCandidatesOnScreen => showCandidatesOnScreen.Value;
    internal static bool ScrollCandidates => scrollCandidates.Value;
    internal static bool ShowWristUI => showWristUI.Value;
    internal static bool SendCandidatesInChannel => sendCandidatesInChannel.Value;
    internal static bool IgnoreRepeatVotes => ignoreRepeatVotes.Value;
    internal static bool ProportionalVoting => proportionalVoting.Value;
    internal static bool EnableRemoteVoting => enableRemoteVoting.Value;
    internal static bool SyncEffects => syncEffects.Value;
    internal static bool UseBagRandomizer => useBagRandomizer.Value;
    internal static List<string> ForceEnabledEffects { get; private set; } = new List<string>();
    internal static List<string> ForceDisabledEffects { get; private set; } = new List<string>();
#if DEBUG
    internal static bool enableIMGUI = false;
    internal static bool IMGUIUseBag = false;
#endif

    internal static void Init()
    {
        category = MelonPreferences.CreateCategory(CATEGORY_NAME);

        MelonPreferences.CreateEntry(CATEGORY_NAME, "token", "YOUR_TOKEN_HERE", "token");
        MelonPreferences.CreateEntry(CATEGORY_NAME, "channel", "CHANNEL_ID_HERE", "channel");
        randomOnNoVotes = category.CreateEntry("randomEffectOnNoVotes", true);
        useGravityEffects = category.CreateEntry("useGravityEffects", false);
        useSteamProfileEffects = category.CreateEntry("useSteamProfileEffects", false);
        useLaggyEffects = category.CreateEntry("useLaggyEffects", false);
        useMetaEffects = category.CreateEntry("useMetaEffects", true);
        // voteprefs
        showCandidatesOnScreen = category.CreateEntry("showCandidatesOnScreen", true);
        scrollCandidates = category.CreateEntry("scrollCandidates", true);
        showWristUI = category.CreateEntry("showWristUI", true);
        sendCandidatesInChannel = category.CreateEntry("sendCandidatesInChannel", false);
        ignoreRepeatVotes = category.CreateEntry("ignoreRepeatVotesFromSameUser", true);
        proportionalVoting = category.CreateEntry("proportionalVoting", true);
        enableRemoteVoting = category.CreateEntry("enableRemoteVoting", false);
        // end voteprefs :^)
        syncEffects = category.CreateEntry("syncEffectsViaEntanglement", false);
        useBagRandomizer = category.CreateEntry("useBagRandomizer", false);
        category.CreateEntry("forceEnabledEffects", ForceEnabledEffects.ToArray());
        category.CreateEntry("forceDisabledEffects", ForceDisabledEffects.ToArray());

#if DEBUG
        MelonPreferences.CreateEntry(CATEGORY_NAME, "enableIMGUI", enableIMGUI, "enableIMGUI");
        MelonPreferences.CreateEntry(CATEGORY_NAME, "IMGUIUseBag", IMGUIUseBag, "IMGUIUseBag");
#endif
        category.SaveToFile();
        category.LoadFromFile();
    }

    public static void Get()
    {
        ForceEnabledEffects = MelonPreferences.GetEntryValue<string[]>(CATEGORY_NAME, "forceEnabledEffects").ToList();
        ForceDisabledEffects = MelonPreferences.GetEntryValue<string[]>(CATEGORY_NAME, "forceDisabledEffects").ToList();
#if DEBUG
        enableIMGUI = MelonPreferences.GetEntryValue<bool>(CATEGORY_NAME, "enableIMGUI");
        IMGUIUseBag = MelonPreferences.GetEntryValue<bool>(CATEGORY_NAME, "IMGUIUseBag");
#endif

        Chaos.eTypesToPrefs.Clear();
        // populate eTypesToPrefs now
        Chaos.eTypesToPrefs.AddRange(new (EffectTypes, bool)[] {
            (EffectTypes.AFFECT_GRAVITY, UseGravityEffects),
            (EffectTypes.AFFECT_STEAM_PROFILE, UseSteamProfileEffects),
            (EffectTypes.USE_STEAM, Chaos.isSteamVer),
            (EffectTypes.LAGGY, UseLaggyEffects),
            (EffectTypes.HIDDEN, true),
            (EffectTypes.DONT_SYNC, !SyncEffects),
            (EffectTypes.META, UseMetaEffects),
            (EffectTypes.DEFAULT_DISABLED, false),
        });
    }

    public static async void SendBotInitalValues()
    {
        await GlobalVariables.WatsonClient.SendAsync("token:" + MelonPreferences.GetEntryValue<string>(CATEGORY_NAME, "token"));
        await GlobalVariables.WatsonClient.SendAsync("channel:" + MelonPreferences.GetEntryValue<string>(CATEGORY_NAME, "channel").Trim());
    }
}
