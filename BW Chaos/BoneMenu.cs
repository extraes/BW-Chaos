using BWChaos.Effects;
using HarmonyLib;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.BoneMenu;
using System;
using System.Collections;
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
        internal static MenuCategory debugCategory;

        public static void Register()
        {
            if (boneMenuEntry == null)
            {
                boneMenuEntry = MenuManager.CreateCategory("Chaos", Color.white);
                boneMenuEntry.CreateFunctionElement("Reset/refilter effects", Color.white, Chaos.LiveUpdateEffects);
                preferencesCategory = boneMenuEntry.CreateSubCategory("Preferences", Color.white);
                effectsCategory = boneMenuEntry.CreateSubCategory("Effects", Color.gray);
                debugCategory = boneMenuEntry.CreateSubCategory("Debug", Color.gray);
            }

            var sorted = Chaos.asmEffects.OrderBy(e => e.Name).ToList();
            foreach (EffectBase effect in sorted)
            {
                // don't let the oculus players activate effects that use steam
                if (Chaos.isSteamVer && effect.Types.HasFlag(EffectTypes.USE_STEAM)) continue;

                var ecat = effectsCategory.CreateSubCategory(effect.Name, Color.white);
                effect.MenuElement = ecat;


                // As usual, make a force runner
                ecat.CreateFunctionElement("Force run", Color.white, () =>
                {
                    var type = effect.GetType();
                    var e = (EffectBase)Activator.CreateInstance(type);
                    e.Run();
                });

                ecat.CreateBoolElement("Force enable/disable", Color.white, EffectHandler.allEffects.ContainsKey(effect.Name), addEffect =>
                {
#if DEBUG
                    Chaos.Log("BoneMenu effect toggle for " + effect.Name + " pressed; Effect is " + (EffectHandler.allEffects.ContainsKey(effect.Name) ? "" : "not ") + "in the list; b == " + addEffect);
#endif
                    if (addEffect)
                    {
                        if (!EffectHandler.allEffects.ContainsKey(effect.Name))
                        {
                            EffectHandler.allEffects.Add(effect.Name, effect);
                            if (!Chaos.IsEffectViable(effect.Types)) Prefs.ForceEnabledEffects.Add(effect.Name);
                        }
                    }
                    else
                    {
                        foreach (var e in GlobalVariables.ActiveEffects)
                        {
                            if (e.Name == effect.Name) e.ForceEnd();
                        }
                        EffectHandler.allEffects.Remove(effect.Name);
                        if (Chaos.IsEffectViable(effect.Types)) Prefs.ForceDisabledEffects.Add(effect.Name);
                    }
                    (ecat.elements[1] as BoolElement).SetValue(EffectHandler.allEffects.ContainsKey(effect.Name)); // fallback cause i almost certainly fucked it
                });

                ecat.CreateFunctionElement("Flags: " + effect.Types, Color.gray, () => { });

                effect.GetPreferencesFromAttrs();
            }

            #region Manually populate bonemenu with MelonPreferences

            // Start the entanglement module, assuming it isn't started already
            if (!Prefs.SyncEffects) preferencesCategory.CreateFunctionElement("Start entanglement module", Color.white, () =>
            {
                // delete this menu element when its ran
                preferencesCategory.elements.Remove(preferencesCategory.elements.FirstOrDefault(e => e.displayText == "Start entanglement module"));
                Extras.EntanglementSyncHandler.Init();
                Prefs.syncEffects.Value = !Prefs.SyncEffects;
                Prefs.syncEffects.Save();
                Chaos.LiveUpdateEffects();
            });

            preferencesCategory.CreateBoolElement("Random on no votes", Color.white, Prefs.RandomOnNoVotes, b =>
            {
                Prefs.randomOnNoVotes.Value = b;
                Prefs.randomOnNoVotes.Save();
            });

            if (Prefs.EnableRemoteVoting) preferencesCategory.CreateBoolElement("Proportional voting", Color.white, Prefs.ProportionalVoting, b =>
            {
                MelonPreferences.SetEntryValue<bool>("BW_Chaos", "ignoreRepeatVotesFromSameUser", b);
                Prefs.ignoreRepeatVotes.Value = b;
                Prefs.ignoreRepeatVotes.Save();
                GlobalVariables.WatsonClient.SendAsync(Encoding.UTF8.GetBytes("ignorerepeatvotes:" + b)).GetAwaiter().GetResult();
            });

            preferencesCategory.CreateBoolElement("Show candidate effects on screen", Color.white, Prefs.ShowCandidatesOnScreen, b =>
            {
                Prefs.showCandidatesOnScreen.Value = b;
                Prefs.showCandidatesOnScreen.Save();
                // this doesn't necessitate reloading effects
            });

            preferencesCategory.CreateBoolElement("Use gravity effects", Color.white, Prefs.UseGravityEffects, b =>
            {
                Prefs.useGravityEffects.Value = b;
                Prefs.useGravityEffects.Save();
                Chaos.LiveUpdateEffects();
            });

            preferencesCategory.CreateBoolElement("Use laggy effects", Color.white, Prefs.UseLaggyEffects, b =>
            {
                Prefs.useLaggyEffects.Value = b;
                Prefs.useLaggyEffects.Save();
                Chaos.LiveUpdateEffects();
            });

            if (Chaos.isSteamVer) preferencesCategory.CreateBoolElement("Use Steam profile effects", Color.white, Prefs.UseSteamProfileEffects, b =>
            {
                Prefs.useSteamProfileEffects.Value = b;
                Prefs.useSteamProfileEffects.Save();
                Chaos.LiveUpdateEffects();
            });

            if (Chaos.isSteamVer) preferencesCategory.CreateBoolElement("Use meta effects", Color.white, Prefs.UseMetaEffects, b =>
            {
                Prefs.useMetaEffects.Value = b;
                Prefs.useMetaEffects.Save();
                Chaos.LiveUpdateEffects();
            });

            if (Chaos.isSteamVer) preferencesCategory.CreateBoolElement("Toggle wrist UI", Color.white, Prefs.ShowWristUI, b =>
            {
                Prefs.showWristUI.Value = b;
                Prefs.showWristUI.Save();
            });

            if (Chaos.isSteamVer) preferencesCategory.CreateBoolElement("Toggle candidates on screen", Color.white, Prefs.ShowCandidatesOnScreen, b =>
            {
                Prefs.showCandidatesOnScreen.Value = b;
                Prefs.showCandidatesOnScreen.Save();
            });

            #endregion

            #region Populate debug category

            debugCategory.CreateFunctionElement("Log resource paths", Color.white, () => { GlobalVariables.ResourcePaths.ForEach(Chaos.Log); });
            debugCategory.CreateFunctionElement("Log all enabled effects", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log(e.Value.Name)); });
            debugCategory.CreateFunctionElement("Log effect syncing indices", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"{e.Value.Name}: {e.Value.EffectIndex}")); });
            debugCategory.CreateFunctionElement("Log effect type names (useful for ChaosConfig)", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"Effect '{e.Value.Name}' = type '{e.Value.GetType().Name}'")); });
            debugCategory.CreateFunctionElement("Log all preferences (w/o token & channel)", Color.white, () =>
            {
                foreach (var prop in typeof(Prefs).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static))
                {
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum || prop.PropertyType == typeof(string))
                    {
                        Chaos.Log($"{prop.Name}: {prop.GetValue(null)}");
                    }
                    else if (prop.PropertyType.GetInterfaces().Any(t => t == typeof(IEnumerable)))
                    {
                        Chaos.Log(prop.Name + ": ");
                        foreach (var item in (IEnumerable)prop)
                        {
                            Chaos.Log(" - " + (item?.ToString() ?? "<null>"));
                        }
                    }
                }
            });

#if DEBUG
            // why put this in chaos? fucking beats me
            Action act = new Action(() => GameObject.FindObjectOfType<HotKeyEnable>().Spawn());
            debugCategory.CreateFunctionElement("UIRig.popUpMenu.addFunMenu", Color.gray, () => GameObject.FindObjectOfType<StressLevelZero.Rig.UIRig>().popUpMenu.AddDevMenu(act));
#endif

            var logMethods = new HarmonyMethod[]
            {
                new HarmonyMethod(typeof(Chaos), nameof(Chaos.Warn), new Type[] { typeof(string) }),
                new HarmonyMethod(typeof(Chaos), nameof(Chaos.Warn), new Type[] { typeof(object) }),
                new HarmonyMethod(typeof(Chaos), nameof(Chaos.Error), new Type[] { typeof(string) }),
                new HarmonyMethod(typeof(Chaos), nameof(Chaos.Error), new Type[] { typeof(object) }),
            };
            var postfixes = new HarmonyMethod[]
            {
                new HarmonyMethod(typeof(BoneMenu), nameof(BoneMenu.WarnPostfix), new Type[] { typeof(string) }),
                new HarmonyMethod(typeof(BoneMenu), nameof(BoneMenu.WarnPostfix), new Type[] { typeof(object) }),
                new HarmonyMethod(typeof(BoneMenu), nameof(BoneMenu.ErrorPostfix), new Type[] { typeof(string) }),
                new HarmonyMethod(typeof(BoneMenu), nameof(BoneMenu.ErrorPostfix), new Type[] { typeof(object) }),
            };
            debugCategory.CreateFunctionElement("Enable warn/error notifications", Color.white, () =>
            {
                for (int i = 0; i < logMethods.Length; i++)
                {
                    var logMethod = logMethods[i];
                    var postfix = postfixes[i];
                    Chaos.Instance.HarmonyInstance.Patch(logMethod.method, null, postfix);
                }
                Notifications.SendNotification("Enabled notifications", 3);
            });

            #endregion
        }

        private static NotificationData lastNotif;
        private static void WarnPostfix(object obj) => WarnPostfix(obj?.ToString() ?? "null");
        private static void WarnPostfix(string str)
        {
            lastNotif?.End();
            lastNotif = Notifications.SendNotification("WARN - " + str, 5, Color.yellow);
        }

        private static void ErrorPostfix(object obj) => ErrorPostfix(obj?.ToString() ?? "null");
        private static void ErrorPostfix(string str)
        {
            lastNotif?.End();
            lastNotif = Notifications.SendNotification("ERROR - " + str, 5, Color.red);
        }
    }
}
