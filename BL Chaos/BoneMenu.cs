#if !NOBONELIB
using BLChaos.Effects;
using static BLChaos.Effects.EffectBase;
using BoneLib.BoneMenu;
using MelonLoader.Utils;

namespace BLChaos;

public static class BoneMenu
{
    internal static Page boneMenuEntry;
    internal static Page recentCategory;
    internal static Page effectsCategory;
    internal static Page preferencesCategory;
    internal static Page debugCategory;
    internal static Page[] effectsCategoriesAlphabetized = new Page[27]; // last one for numbers

    internal static Page GetPageFor(string name)
    {
        char firstAlphanumericChar = name.First(c => char.IsLetterOrDigit(c));

        char firstCharUpper = char.ToUpper(firstAlphanumericChar);
        int alphabetizedIdx = char.IsDigit(firstCharUpper) ? effectsCategoriesAlphabetized.Length - 1 : firstCharUpper.CompareTo('A');
        if (effectsCategoriesAlphabetized[alphabetizedIdx] == null)
            effectsCategoriesAlphabetized[alphabetizedIdx] = effectsCategory.CreatePage("Effects starting with " + (char.IsDigit(firstCharUpper) ? "a number" : firstCharUpper), Color.white);

        PageLinkElement? link = effectsCategoriesAlphabetized[alphabetizedIdx].Elements.FirstOrDefault(mc => mc.ElementName == name) as PageLinkElement;
        Page? ret = link?.LinkedPage;
        
        ret ??= effectsCategoriesAlphabetized[alphabetizedIdx].CreatePage(name, Color.white); // create if doesnt exist
        
        return ret;
    }

    public static void Register()
    {
        Chaos.OnEffectRan += UpdateRecentEffect;

        if (boneMenuEntry == null)
        {
            boneMenuEntry = Page.Root.CreatePage("Chaos", Color.white);
            recentCategory = boneMenuEntry.CreatePage("Recent Effects", Color.white);
            boneMenuEntry.CreateFunction("Reset/refilter effects", Color.white, Chaos.LiveUpdateEffects);
            preferencesCategory = boneMenuEntry.CreatePage("Preferences", Color.white);
            effectsCategory = boneMenuEntry.CreatePage("Effects", Color.gray);
            debugCategory = boneMenuEntry.CreatePage("Debug", Color.gray);
        }

        System.Collections.Generic.List<EffectBase> sorted = Chaos.asmEffects.OrderBy(e => e.Name).ToList();
        foreach (EffectBase effect in sorted)
        {
            // don't let the oculus players activate effects that use steam
            if (Chaos.isSteamVer && effect.Types.HasFlag(EffectTypes.USE_STEAM)) continue;
            
            // way overbuilt method of sorting categories. this could (should) be abstracted out into another method.
            char firstChar = char.ToUpper(effect.Name.First(char.IsLetterOrDigit));
            int idx = char.IsDigit(firstChar) ? effectsCategoriesAlphabetized.Length - 1 : firstChar.CompareTo('A');
            if (effectsCategoriesAlphabetized[idx] == null)
                effectsCategoriesAlphabetized[idx] = effectsCategory.CreatePage("Effects starting with " + (char.IsDigit(firstChar) ? "a number" : firstChar), Color.white);
            
            Page effectPage = effectsCategoriesAlphabetized[idx].CreatePage(effect.Name, Color.white);
            effect.Page = effectPage;

            // As usual, make a force runner
            effectPage.CreateFunction("Force run", Color.white, () =>
            {
                Type type = effect.GetType();
                EffectBase e = Activator.CreateInstance(type) as EffectBase ?? throw new InvalidCastException($"Type {type.FullName} does not extend EffectBase");
                e.Run();
                Stats.EffectCalledManuallyCallback(effect);
            });

            effectPage.CreateBool("Force enable/disable", Color.white, EffectHandler.allEffects.ContainsKey(effect.Name), addEffect =>
            {
#if DEBUG
                Chaos.Log("BoneMenu effect toggle for " + effect.Name + " pressed; Effect is currently " + (EffectHandler.allEffects.ContainsKey(effect.Name) ? "" : "not ") + "in the list; b == " + addEffect);
#endif
                if (addEffect)
                {
                    if (!EffectHandler.allEffects.ContainsKey(effect.Name))
                    {
                        EffectHandler.allEffects.Add(effect.Name, effect);
                        if (!Chaos.IsEffectAllowed(effect.Types)) Prefs.ForceEnabledEffects.Add(effect.Name);
                    }
                }
                else
                {
                    foreach (EffectBase e in GlobalVariables.ActiveEffects)
                    {
                        if (e.Name == effect.Name) e.ForceEnd();
                    }
                    EffectHandler.allEffects.Remove(effect.Name);
                    if (Chaos.IsEffectAllowed(effect.Types)) Prefs.ForceDisabledEffects.Add(effect.Name);
                }
                //(ecat.Elements[1] as BoolElement).set(EffectHandler.allEffects.ContainsKey(effect.Name)); // fallback cause i almost certainly fucked it
            });

            effectPage.CreateFunction("Flags: " + effect.Types, Color.gray, () => { });

            effect.RegisterPreferences();
        }
        List<Element> elementsCopy = effectsCategory.Elements.ToList();
        elementsCopy.Sort((me1, me2) => StringComparer.InvariantCultureIgnoreCase.Compare(me1.ElementName, me2.ElementName));
        effectsCategory.RemoveAll();

        foreach (Element item in elementsCopy)
            effectsCategory.Add(item);

        #region Populate debug category

        debugCategory.CreateFunction("Log resource paths", Color.white, () => { GlobalVariables.ResourcePaths.ForEach(Chaos.Log); });
        debugCategory.CreateFunction("Log all enabled effects", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log(e.Value.Name)); });
        debugCategory.CreateFunction("Log effect syncing indices", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"{e.Value.Name}: {e.Value.EffectIndex}")); });
        debugCategory.CreateFunction("Log effect type names (useful for ChaosConfig)", Color.white, () => { EffectHandler.allEffects.ForEach(e => Chaos.Log($"Effect '{e.Value.Name}' = type '{e.Value.GetType().Name}'")); });

#if DEBUG
        debugCategory.CreateFunction("Run all tests", Color.white, async () =>
        {
            string basePath = Path.Combine(MelonEnvironment.UserDataDirectory, "Chaos", "TestResult");
            string chaosDir = Path.GetDirectoryName(basePath) ?? throw new DirectoryNotFoundException($"Path {basePath} is not in a directory!");

            if (!Directory.Exists(chaosDir)) Directory.CreateDirectory(chaosDir);
            
            Vector3 startPos = GlobalVariables.Player_RigManager.transform.position;
            foreach (var kvp in EffectHandler.allEffects)
            {
                GlobalVariables.Player_RigManager.Teleport(startPos, true);
                //todo: finish
            }
        });
#endif
        debugCategory.CreateFunction("Log all preferences (w/o token & channel)", Color.white, () =>
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
        Element? pseudoLink =  recentCategory.Elements.FirstOrDefault(el => el.ElementName == effect.Page.Name);
        if (pseudoLink is not null)
            recentCategory.Remove(pseudoLink);
        else
            pseudoLink = recentCategory.CreateFunction(effect.Page.Name, Color.white, () => Menu.OpenPage(effect.Page));
        
        List<Element> copies = recentCategory.Elements.ToList();
        copies.Insert(0, pseudoLink);
        foreach (Element item in copies)
            recentCategory.Add(pseudoLink);
    }
}
#endif