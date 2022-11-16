using Jevil.Prefs;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using static BLChaos.Effects.EffectBase;

namespace BLChaos;

[Preferences(CATEGORY_NAME, true)]
[PreferencesColor(UnityDefaultColor.YELLOW)]
public static class Prefs
{
    internal const string CATEGORY_NAME = "Chaos";
    private static MelonPreferences_Category category;


    [Pref("Toggles effects being ran when no votes are recieved. Enable this if you want to play w/o the Discord/Twitch bot.")]
    internal static bool randomOnNoVotes = true;
    [Pref("Toggles computationally intensive effects. These may tank your framerate.")]
    internal static bool useLaggyEffects = false;
    [Pref("Toggles effects that modify gravity. These are hectic and may crash the game.")]
    internal static bool useGravityEffects = false;
    [Pref("Toggles ONE effect that changes your steam username.")]
    internal static bool useSteamProfileEffects = false;
    [Pref("Toggles effects that change the way this mod works.")]
    internal static bool useMetaEffects = true;
    [Pref("Shows effect candidates on the flatscreen UI.")]
    internal static bool showCandidatesOnScreen = true;
    [Pref("Toggles an animation that occurs when the timer runs out & switches effects.")]
    internal static bool scrollCandidates = true;
    [Pref]
    internal static bool showWristUI = true;
    [Pref("Only used in the DISCORD bot - Sends the candidate effects in the discord channel.")]
    internal static bool sendCandidatesInChannel = true;
    [Pref("Only used in the Disc/Twitch bot - Ignores repeat votes from the same user.")]
    internal static bool ignoreRepeatVotes = true;
    [Pref("Only used in the Disc/Twitch bot, if false: majority wins.")]
    internal static bool proportionalVoting = true;
    [Pref("Enables the Discord/Twitch bot.")]
    internal static bool enableRemoteVoting = false;
    //[Pref("Doesn't work - BONELAB Fusion isn't out.")]
    internal const bool syncEffects = false; // todo: change to static instead of const, uncomment attribute
    [Pref("Keeps a list of effects that have been ran and doesn't run effects twice. Makes sure (nearly) all effects are ran before resetting the list.")]
    internal static bool useBagRandomizer = true;
    [Pref("Slowly changes the amount of time each effect takes over time. Only changes when an effect is ran.")]
    internal static bool modulateEffectTime = false;
    [Pref("Runs an effect on level load. You can use MassEffect to run multiple effects at once.")]
    internal static string effectOnSceneLoad = "";
    [RangePref(1, 15, 1)] internal static int MaxActiveEffects = 10;
    internal static List<string> ForceEnabledEffects { get; private set; } = new List<string>();
    internal static List<string> ForceDisabledEffects { get; private set; } = new List<string>();
#if DEBUG
    internal static bool enableIMGUI = false;
    internal static bool IMGUIUseBag = false;
#endif

    internal static void Init()
    {
        PrefEntries entries = Preferences.Register(typeof(Prefs));
        category = MelonPreferences.CreateCategory(CATEGORY_NAME);

        MelonPreferences.CreateEntry(CATEGORY_NAME, "token", "YOUR_TOKEN_HERE", "token", "If using remote voting: Discord/Twitch token");
        MelonPreferences.CreateEntry(CATEGORY_NAME, "channel", "CHANNEL_ID_HERE", "channel", "If using remote voting: Discord channel ID/Twitch channel name.");
        category.CreateEntry("forceEnabledEffects", ForceEnabledEffects.ToArray());
        category.CreateEntry("forceDisabledEffects", ForceDisabledEffects.ToArray());

#if DEBUG
        category.CreateEntry("enableIMGUI", enableIMGUI, "enableIMGUI");
        category.CreateEntry("IMGUIUseBag", IMGUIUseBag, "IMGUIUseBag");
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
            (EffectTypes.AFFECT_GRAVITY, useGravityEffects),
            (EffectTypes.AFFECT_STEAM_PROFILE, useSteamProfileEffects),
            (EffectTypes.USE_STEAM, Chaos.isSteamVer),
            (EffectTypes.LAGGY, useLaggyEffects),
            (EffectTypes.HIDDEN, true),
            (EffectTypes.DONT_SYNC, !syncEffects),
            (EffectTypes.META, useMetaEffects),
            (EffectTypes.DEFAULT_DISABLED, false),
        });
    }

    public static async void SendBotInitalValues()
    {
        await GlobalVariables.WatsonClient.SendAsync("token:" + MelonPreferences.GetEntryValue<string>(CATEGORY_NAME, "token"));
        await GlobalVariables.WatsonClient.SendAsync("channel:" + MelonPreferences.GetEntryValue<string>(CATEGORY_NAME, "channel").Trim());
    }
}
