using MelonLoader;
using MelonLoader.Utils;

namespace BLChaos;

internal static class EffectConfig
{
    static readonly string Path = System.IO.Path.Join(MelonEnvironment.UserDataDirectory, "ChaosConfig.cfg");
    static readonly Dictionary<string, MelonPreferences_Category> categories = new Dictionary<string, MelonPreferences_Category>();
    public static IReadOnlyList<MelonPreferences_Category> rawCategories => categories.Values.ToList().AsReadOnly();

    public static MelonPreferences_Category GetCategory(string identifier)
    {
        if (categories.TryGetValue(identifier, out MelonPreferences_Category? cat)) return cat;
        else throw new KeyNotFoundException("Effect category not found - " + identifier);
    }

    public static MelonPreferences_Category CreateCategory(string identifier)
    {
        MelonPreferences_Category cat = MelonPreferences.CreateCategory(identifier);
        categories.Add(identifier, cat);
        cat.SetFilePath(Path);
        cat.LoadFromFile(false);
        return cat;
    }

    public static MelonPreferences_Category GetOrCreateCategory(string identifier)
    {
        if (categories.TryGetValue(identifier, out MelonPreferences_Category? cat)) return cat;
        else
        {
            MelonPreferences_Category categ = MelonPreferences.CreateCategory(identifier);
            categories.Add(identifier, categ);
            categ.SetFilePath(Path, true, false);
            return categ;
        }
    }
}
