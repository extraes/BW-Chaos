using BWChaos.Effects;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using System.Linq;
using System.Text;
using UnityEngine;
using static BWChaos.Effects.EffectBase;

namespace BWChaos
{
    // Use partial to stay in scope, because leaving it is a BITCH to deal with.
    public partial class BWChaos : MelonMod
    {
        internal static MenuCategory boneMenuEntry;
        internal static MenuCategory effectsCategory;
        internal static MenuCategory preferencesCategory;

        private void RegisterBoneMenu()
        {
            if (boneMenuEntry == null)
            {
                boneMenuEntry = MenuManager.CreateCategory("Chaos", Color.white);
                boneMenuEntry.CreateFunctionElement("Reset/refilter effects", Color.white, LiveUpdateEffects);
                preferencesCategory = boneMenuEntry.CreateSubCategory("Preferences", Color.white);
                effectsCategory = boneMenuEntry.CreateSubCategory("Effects", Color.gray);
            }

            foreach (EffectBase effect in asmEffects)
            {
                // don't let the oculus players activate effects that use steam
                if (isSteamVer && effect.Types.HasFlag(EffectTypes.USE_STEAM)) continue;

                var ecat = effectsCategory.CreateSubCategory(effect.Name, Color.white);

                ecat.CreateFunctionElement("Force run", Color.white, () => effect.Run());
                ecat.CreateBoolElement("Force enable/disable", Color.white, EffectHandler.AllEffects.ContainsKey(effect.Name), addEffect =>
                {
#if DEBUG
                    MelonLogger.Msg("BoneMenu effect toggle for " + effect.Name + " pressed; Effect is " + (EffectHandler.AllEffects.ContainsKey(effect.Name) ? "" : "not ") + "in the list; b == " + addEffect);
#endif
                    if (addEffect)
                    {
                        if (!EffectHandler.AllEffects.ContainsKey(effect.Name))
                            EffectHandler.AllEffects.Add(effect.Name, effect);
                    }
                    else
                    {
                        effect.ForceEnd();
                        EffectHandler.AllEffects.Remove(effect.Name);
                    }
                    (ecat.elements[1] as BoolElement).SetValue(EffectHandler.AllEffects.ContainsKey(effect.Name)); // fallback cause i almost certainly fucked it
                });

            }
            #region Manually populate bonemenu with MelonPreferences

            if (!syncEffects) preferencesCategory.CreateFunctionElement("Start entanglement module", Color.white, () =>
            {
                preferencesCategory.elements.Remove(preferencesCategory.elements.FirstOrDefault(e => e.displayText == "Start entanglement module"));
                Extras.EntanglementSyncHandler.Init();
            });

            preferencesCategory.CreateBoolElement("Random on no votes", Color.white, EffectHandler.randomOnNoVotes, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes", b);
                MelonPreferences.Save();
                GetMelonPreferences();
            });

            if (enableRemoteVoting) preferencesCategory.CreateBoolElement("Proportional voting", Color.white, proportionalVoting, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser", b);
                GlobalVariables.WatsonClient.SendAsync(Encoding.UTF8.GetBytes("ignorerepeatvotes:" + b)).GetAwaiter().GetResult();
                MelonPreferences.Save();
            });

            preferencesCategory.CreateBoolElement("Show candidate effects on screen", Color.white, showCandidatesOnScreen, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "showCandidatesOnScreen", b);
                MelonPreferences.Save();
                GetMelonPreferences(); // this doesn't necessitate reloading effects
            });

            preferencesCategory.CreateBoolElement("Use gravity effects", Color.white, useGravityEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useGravityEffects", b);
                MelonPreferences.Save();
                LiveUpdateEffects();
            });

            preferencesCategory.CreateBoolElement("Use laggy effects", Color.white, useLaggyEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useLaggyEffects", b);
                MelonPreferences.Save();
                LiveUpdateEffects();
            });

            if (isSteamVer) preferencesCategory.CreateBoolElement("Use Steam profile effects", Color.white, useSteamProfileEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects", b);
                MelonPreferences.Save();
                LiveUpdateEffects();
            });

            #endregion
        }
    }
}
/*
public bool randomOnNoVotes = false;
internal static bool useLaggyEffects = false;
internal static bool useGravityEffects = false;
internal static bool useSteamProfileEffects = false;
internal static bool ignoreRepeatVotes = false;
internal static bool proportionalVoting = true;
internal static bool enableDiscordVoting = false;
internal static bool syncEffects = false;

MelonPreferences.CreateEntry("BW_Chaos", "randomEffectOnNoVotes", randomOnNoVotes, "randomEffectOnNoVotes");
randomOnNoVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes");

MelonPreferences.CreateEntry("BW_Chaos", "useGravityEffects", useGravityEffects, "useGravityEffects");
useGravityEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useGravityEffects");

MelonPreferences.CreateEntry("BW_Chaos", "useSteamProfileEffects", useSteamProfileEffects, "useSteamProfileEffects");
useSteamProfileEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects");

MelonPreferences.CreateEntry("BW_Chaos", "useLaggyEffects", useLaggyEffects, "useLaggyEffects");
useLaggyEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "useLaggyEffects");

// Voting preferences
MelonPreferences.CreateEntry("BW_Chaos", "ignoreRepeatVotesFromSameUser", ignoreRepeatVotes, "ignoreRepeatVotesFromSameUser");
ignoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser");

MelonPreferences.CreateEntry("BW_Chaos", "proportionalVoting", proportionalVoting, "proportionalVoting");
ignoreRepeatVotes = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "proportionalVoting");

MelonPreferences.CreateEntry("BW_Chaos", "enableDiscordVoting", enableDiscordVoting, "enableDiscordVoting");
enableDiscordVoting = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "enableDiscordVoting");
// yeah what that comment said

MelonPreferences.CreateEntry("BW_Chaos", "syncEffectsViaEntanglement", syncEffects, "syncEffectsViaEntanglement");
syncEffects = MelonPreferences.GetEntryValue<bool>("BW_Chaos", "syncEffectsViaEntanglement");
*/