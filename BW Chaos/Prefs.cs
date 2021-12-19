using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using static BWChaos.Effects.EffectBase;

namespace BWChaos
{
    public static class Prefs
    {
        internal static bool UseLaggyEffects { get; private set; } = false;
        internal static bool UseGravityEffects { get; private set; } = false;
        internal static bool UseSteamProfileEffects { get; private set; } = false;
        internal static bool UseMetaEffects { get ; private set; } = true;
        internal static bool ShowCandidatesOnScreen { get; private set; } = true;
        internal static bool ShowWristUI { get; private set; } = true;
        internal static bool SendCandidatesInChannel { get; private set; } = true;
        internal static bool IgnoreRepeatVotes { get; private set; } = false;
        internal static bool ProportionalVoting { get; private set; } = true;
        internal static bool EnableRemoteVoting { get; private set; } = false;
        internal static bool SyncEffects { get; private set; } = false;
        internal static List<string> ForceEnabledEffects { get; private set; } = new List<string>();
        internal static List<string> ForceDisabledEffects { get; private set; } = new List<string>();
#if DEBUG
        internal static bool enableIMGUI = false;
#endif

        internal static void Init()
        {
            MelonPreferences.CreateEntry("BW_Chaos", "token", "YOUR_TOKEN_HERE", "token");
            MelonPreferences.CreateEntry("BW_Chaos", "channel", "CHANNEL_ID_HERE", "channel");
            MelonPreferences.CreateEntry("BW_Chaos", "randomEffectOnNoVotes", EffectHandler.randomOnNoVotes, "randomEffectOnNoVotes");
            MelonPreferences.CreateEntry("BW_Chaos", "useGravityEffects", UseGravityEffects, "useGravityEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useSteamProfileEffects", UseSteamProfileEffects, "useSteamProfileEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useLaggyEffects", UseLaggyEffects, "useLaggyEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "useMetaEffects", UseMetaEffects, "useMetaEffects");
            // voteprefs
            MelonPreferences.CreateEntry("BW_Chaos", "showCandidatesOnScreen", ShowCandidatesOnScreen, "showCandidatesOnScreen");
            MelonPreferences.CreateEntry("BW_Chaos", "showWristUI", ShowWristUI, "showWristUI");
            MelonPreferences.CreateEntry("BW_Chaos", "sendCandidatesInChannel", SendCandidatesInChannel, "sendCandidatesInChannel");
            MelonPreferences.CreateEntry("BW_Chaos", "ignoreRepeatVotesFromSameUser", IgnoreRepeatVotes, "ignoreRepeatVotesFromSameUser");
            MelonPreferences.CreateEntry("BW_Chaos", "proportionalVoting", ProportionalVoting, "proportionalVoting");
            MelonPreferences.CreateEntry("BW_Chaos", "enableRemoteVoting", EnableRemoteVoting, "enableRemoteVoting");
            // end voteprefs :^)
            MelonPreferences.CreateEntry("BW_Chaos", "syncEffectsViaEntanglement", SyncEffects, "syncEffectsViaEntanglement");
            MelonPreferences.CreateEntry("BW_Chaos", "forceEnabledEffects", ForceEnabledEffects.ToArray(), "forceEnabledEffects");
            MelonPreferences.CreateEntry("BW_Chaos", "forceDisabledEffects", ForceDisabledEffects.ToArray(), "forceDisabledEffects");
#if DEBUG
            MelonPreferences.CreateEntry("BW_Chaos", "enableIMGUI", enableIMGUI, "enableIMGUI");
#endif
            MelonPreferences.Save();
        }

        public static void Get()
        {
            EffectHandler.randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");
            UseGravityEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useGravityEffects");
            UseSteamProfileEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects");
            UseLaggyEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useLaggyEffects");
            // Voting preferences
            ShowCandidatesOnScreen = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "showCandidatesOnScreen");
            ShowWristUI = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "showWristUI");
            SendCandidatesInChannel = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "sendCandidatesInChannel");
            IgnoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser");
            ProportionalVoting = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "proportionalVoting");
            EnableRemoteVoting = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enableRemoteVoting");
            // yeah what that comment said
            SyncEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "syncEffectsViaEntanglement");
            ForceEnabledEffects = MelonPreferences.GetEntryValue<string[]>("BW_Chaos", "forceEnabledEffects").ToList();
            ForceDisabledEffects = MelonPreferences.GetEntryValue<string[]>("BW_Chaos", "forceDisabledEffects").ToList();
#if DEBUG
            enableIMGUI = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enableIMGUI");
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
            });
        }

        public static async void SendBotInitalValues()
        {
            await GlobalVariables.WatsonClient.SendAsync("token:" + MelonPreferences.GetEntryValue<string>("BW_Chaos", "token"));
            await GlobalVariables.WatsonClient.SendAsync("channel:" + MelonPreferences.GetEntryValue<string>("BW_Chaos", "channel").Trim());
        }
    }
}
