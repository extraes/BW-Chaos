#if !NOBONELIB
using BLChaos.Effects;
using HarmonyLib;
using MelonLoader;
using BoneLib;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Jevil.Prefs;
using static BLChaos.Effects.EffectBase;
using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;
using System.IO;
using Il2CppMK.Glow;

namespace BLChaos;

public static class BoneMenu
{
    internal static MenuCategory boneMenuEntry;
    internal static MenuCategory recentCategory;
    internal static MenuCategory effectsCategory;
    internal static MenuCategory preferencesCategory;
    internal static MenuCategory debugCategory;
    internal static MenuCategory[] effectsCategoriesAlphabetized = new MenuCategory[27]; // last one for numbers

    internal static MenuCategory GetMenuCategoryFor(string name)
    {
        char firstAlphanumericChar = name.First(c => char.IsLetterOrDigit(c));

        char firstCharUpper = char.ToUpper(firstAlphanumericChar);
        int alphabetizedIdx = char.IsDigit(firstCharUpper) ? effectsCategoriesAlphabetized.Length - 1 : firstCharUpper.CompareTo('A');
        if (effectsCategoriesAlphabetized[alphabetizedIdx] == null)
            effectsCategoriesAlphabetized[alphabetizedIdx] = effectsCategory.CreateCategory("Effects starting with " + (char.IsDigit(firstCharUpper) ? "a number" : firstCharUpper), Color.white);

        MenuCategory ret = (MenuCategory)effectsCategoriesAlphabetized[alphabetizedIdx].Elements.FirstOrDefault(mc => mc.Name == name);
        
        ret ??= effectsCategoriesAlphabetized[alphabetizedIdx].CreateCategory(name, Color.white); // create if doesnt exist
        
        return ret;
    }

    public static void Register()
    {
        Chaos.OnEffectRan += UpdateRecentEffect;

        if (boneMenuEntry == null)
        {
            boneMenuEntry = MenuManager.CreateCategory("Chaos", Color.white);
            recentCategory = boneMenuEntry.CreateCategory("Recent Effects", Color.white);
            boneMenuEntry.CreateFunctionElement("Reset/refilter effects", Color.white, Chaos.LiveUpdateEffects);
            preferencesCategory = boneMenuEntry.CreateCategory("Preferences", Color.white);
            effectsCategory = boneMenuEntry.CreateCategory("Effects", Color.gray);
            debugCategory = boneMenuEntry.CreateCategory("Debug", Color.gray);
        }

        System.Collections.Generic.List<EffectBase> sorted = Chaos.asmEffects.OrderBy(e => e.Name).ToList();
        foreach (EffectBase effect in sorted)
        {
            // don't let the oculus players activate effects that use steam
            if (Chaos.isSteamVer && effect.Types.HasFlag(EffectTypes.USE_STEAM)) continue;
            
            // way overbuilt method of sorting categories. this could (should) be abstracted out into another method.
            char firstChar = char.ToUpper(effect.Name.First(c => char.IsLetterOrDigit(c)));
            int idx = char.IsDigit(firstChar) ? effectsCategoriesAlphabetized.Length - 1 : firstChar.CompareTo('A');
            if (effectsCategoriesAlphabetized[idx] == null)
                effectsCategoriesAlphabetized[idx] = effectsCategory.CreateCategory("Effects starting with " + (char.IsDigit(firstChar) ? "a number" : firstChar), Color.white);
            
            MenuCategory ecat = effectsCategoriesAlphabetized[idx].CreateCategory(effect.Name, Color.white);
            effect.MenuElement = ecat;

            // As usual, make a force runner
            ecat.CreateFunctionElement("Force run", Color.white, () =>
            {
                Type type = effect.GetType();
                EffectBase e = (EffectBase)Activator.CreateInstance(type);
                e.Run();
                Stats.EffectCalledManuallyCallback(effect);
            });

            ecat.CreateBoolElement("Force enable/disable", Color.white, EffectHandler.allEffects.ContainsKey(effect.Name), addEffect =>
            {
#if DEBUG
                Chaos.Log("BoneMenu effect toggle for " + effect.Name + " pressed; Effect is currently " + (EffectHandler.allEffects.ContainsKey(effect.Name) ? "" : "not ") + "in the list; b == " + addEffect);
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
                    foreach (EffectBase e in GlobalVariables.ActiveEffects)
                    {
                        if (e.Name == effect.Name) e.ForceEnd();
                    }
                    EffectHandler.allEffects.Remove(effect.Name);
                    if (Chaos.IsEffectViable(effect.Types)) Prefs.ForceDisabledEffects.Add(effect.Name);
                }
                //(ecat.Elements[1] as BoolElement).set(EffectHandler.allEffects.ContainsKey(effect.Name)); // fallback cause i almost certainly fucked it
            });

            ecat.CreateFunctionElement("Flags: " + effect.Types, Color.gray, () => { });

            effect.RegisterPreferences();
        }
        effectsCategory.Elements.Sort((me1, me2) => StringComparer.InvariantCultureIgnoreCase.Compare(me1.Name, me2.Name));

        #region Populate debug category

        debugCategory.CreateFunctionElement("Log resource paths", Color.white, () => { GlobalVariables.ResourcePaths.ForEach(Chaos.Log); });
        debugCategory.CreateFunctionElement("Log all enabled effects", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log(e.Value.Name)); });
        debugCategory.CreateFunctionElement("Log effect syncing indices", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"{e.Value.Name}: {e.Value.EffectIndex}")); });
        debugCategory.CreateFunctionElement("Log effect type names (useful for ChaosConfig)", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"Effect '{e.Value.Name}' = type '{e.Value.GetType().Name}'")); });

#if DEBUG
        debugCategory.CreateFunctionElement("Run all tests", Color.white, async () =>
        {
            string basePath = Path.Combine(MelonUtils.UserDataDirectory, "Chaos", "TestResult");
            string chaosDir = Path.GetDirectoryName(basePath);

            if (!Directory.Exists(chaosDir)) Directory.CreateDirectory(chaosDir);
            
            Vector3 startPos = GlobalVariables.Player_RigManager.transform.position;
            foreach (var kvp in EffectHandler.allEffects)
            {
                GlobalVariables.Player_RigManager.Teleport(startPos, true);
                //todo: finish
            }
        });
#endif
        debugCategory.CreateFunctionElement("Log all preferences (w/o token & channel)", Color.white, () =>
        {
            foreach (System.Reflection.PropertyInfo prop in typeof(Prefs).GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static))
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum || prop.PropertyType == typeof(string))
                {
                    Chaos.Log($"{prop.Name}: {prop.GetValue(null)}");
                }
                else if (prop.GetValue(null) is IEnumerable enumerable)
                {
                    Chaos.Log(prop.Name + ": ");
                    foreach (object item in enumerable)
                    {
                        Chaos.Log(" - " + (item?.ToString() ?? "<null>"));
                    }
                }
            }
        });

        #endregion
    }

    // shortcut to recently ran effects
    private static void UpdateRecentEffect(EffectBase effect)
    {
        recentCategory.Elements.Remove(effect.MenuElement);
        recentCategory.Elements.Add(effect.MenuElement);
    }
}
#endif