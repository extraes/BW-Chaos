using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(BW_Chaos.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(BW_Chaos.BuildInfo.Company)]
[assembly: AssemblyProduct(BW_Chaos.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BW_Chaos.BuildInfo.Author)]
[assembly: AssemblyTrademark(BW_Chaos.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(BW_Chaos.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BW_Chaos.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(BW_Chaos.BW_Chaos), BW_Chaos.BuildInfo.Name, BW_Chaos.BuildInfo.Version, BW_Chaos.BuildInfo.Author, BW_Chaos.BuildInfo.DownloadLink)]
[assembly: VerifyLoaderVersion(0, 4, 2, true)]
[assembly: MelonOptionalDependencies("Steamworks")]

// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]