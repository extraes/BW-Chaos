using BWChaos.Effects;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using System.Linq;
using System.Text;
using UnityEngine;
using static BWChaos.Effects.EffectBase;

namespace BWChaos
{
    public static class BoneMenu
    {
        internal static MenuCategory boneMenuEntry;
        internal static MenuCategory effectsCategory;
        internal static MenuCategory preferencesCategory;

        public static void Register()
        {
            if (boneMenuEntry == null)
            {
                boneMenuEntry = MenuManager.CreateCategory("Chaos", Color.white);
                boneMenuEntry.CreateFunctionElement("Reset/refilter effects", Color.white, Chaos.LiveUpdateEffects);
                preferencesCategory = boneMenuEntry.CreateSubCategory("Preferences", Color.white);
                effectsCategory = boneMenuEntry.CreateSubCategory("Effects", Color.gray);
            }

            foreach (EffectBase effect in Chaos.asmEffects)
            {
                // don't let the oculus players activate effects that use steam
                if (Chaos.isSteamVer && effect.Types.HasFlag(EffectTypes.USE_STEAM)) continue;

                var ecat = effectsCategory.CreateSubCategory(effect.Name, Color.white);
                effect.MenuElement = ecat;

                // As usual, make a force runner
                ecat.CreateFunctionElement("Force run", Color.white, () => effect.Run());

                ecat.CreateBoolElement("Force enable/disable", Color.white, EffectHandler.AllEffects.ContainsKey(effect.Name), addEffect =>
                {
#if DEBUG
                    MelonLogger.Msg("BoneMenu effect toggle for " + effect.Name + " pressed; Effect is " + (EffectHandler.AllEffects.ContainsKey(effect.Name) ? "" : "not ") + "in the list; b == " + addEffect);
#endif
                    if (addEffect)
                    {
                        if (!EffectHandler.AllEffects.ContainsKey(effect.Name))
                        {
                            EffectHandler.AllEffects.Add(effect.Name, effect);
                            if(!Chaos.IsEffectViable(effect.Types)) Prefs.ForceEnabledEffects.Add(effect.Name);
                        }
                    }
                    else
                    {
                        effect.ForceEnd();
                        EffectHandler.AllEffects.Remove(effect.Name);
                        if(Chaos.IsEffectViable(effect.Types)) Prefs.ForceDisabledEffects.Add(effect.Name);
                    }
                    (ecat.elements[1] as BoolElement).SetValue(EffectHandler.AllEffects.ContainsKey(effect.Name)); // fallback cause i almost certainly fucked it
                });

            }
            #region Manually populate bonemenu with MelonPreferences

            // Start the entanglement module, assuming it isn't started already
            if (!Prefs.SyncEffects) preferencesCategory.CreateFunctionElement("Start entanglement module", Color.white, () =>
            {
                // delete this menu element when its ran
                preferencesCategory.elements.Remove(preferencesCategory.elements.FirstOrDefault(e => e.displayText == "Start entanglement module"));
                Extras.EntanglementSyncHandler.Init();
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "syncEffectsViaEntanglement", !Prefs.SyncEffects);
                MelonPreferences.Save();
                Chaos.LiveUpdateEffects();
            });

            preferencesCategory.CreateBoolElement("Random on no votes", Color.white, EffectHandler.randomOnNoVotes, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "randomEffectOnNoVotes", b);
                MelonPreferences.Save();
                Prefs.Get();
            });

            if (Prefs.EnableRemoteVoting) preferencesCategory.CreateBoolElement("Proportional voting", Color.white, Prefs.ProportionalVoting, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser", b);
                GlobalVariables.WatsonClient.SendAsync(Encoding.UTF8.GetBytes("ignorerepeatvotes:" + b)).GetAwaiter().GetResult();
                MelonPreferences.Save();
            });

            preferencesCategory.CreateBoolElement("Show candidate effects on screen", Color.white, Prefs.ShowCandidatesOnScreen, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "showCandidatesOnScreen", b);
                MelonPreferences.Save();
                Prefs.Get(); // this doesn't necessitate reloading effects
            });

            preferencesCategory.CreateBoolElement("Use gravity effects", Color.white, Prefs.UseGravityEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useGravityEffects", b);
                MelonPreferences.Save();
                Chaos.LiveUpdateEffects();
            });

            preferencesCategory.CreateBoolElement("Use laggy effects", Color.white, Prefs.UseLaggyEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useLaggyEffects", b);
                MelonPreferences.Save();
                Chaos.LiveUpdateEffects();
            });

            if (Chaos.isSteamVer) preferencesCategory.CreateBoolElement("Use Steam profile effects", Color.white, Prefs.UseSteamProfileEffects, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "useSteamProfileEffects", b);
                MelonPreferences.Save();
                Chaos.LiveUpdateEffects();
            });

            #endregion
        }
    }
}
