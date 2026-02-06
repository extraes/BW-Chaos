using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[assembly: AssemblyTitle(BLChaos.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(BLChaos.BuildInfo.Company)]
[assembly: AssemblyProduct(BLChaos.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BLChaos.BuildInfo.Author)]
[assembly: AssemblyTrademark(BLChaos.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(BLChaos.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BLChaos.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonPriority(-1)] // load AFTER Fusion (which is -10000) but BEFORE UnityExplorer (has no melonpriority, so 0)

[assembly: MelonInfo(typeof(BLChaos.Chaos), BLChaos.BuildInfo.Name, BLChaos.BuildInfo.Version, BLChaos.BuildInfo.Author, BLChaos.BuildInfo.DownloadLink)]
//[assembly: MelonColor(System.ConsoleColor.Yellow)]
// [assembly: VerifyLoaderVersion("0.5.3", true)] // theres nothing to really warrant it aside from the comments in chaosconfig
[assembly: MelonOptionalDependencies("Facepunch.Steamworks")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
// [assembly: Ungovernable((UngovernableType)~0)]
[assembly: RequiresPreviewFeatures]