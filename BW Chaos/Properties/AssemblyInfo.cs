using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(BWChaos.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(BWChaos.BuildInfo.Company)]
[assembly: AssemblyProduct(BWChaos.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BWChaos.BuildInfo.Author)]
[assembly: AssemblyTrademark(BWChaos.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(BWChaos.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BWChaos.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: MelonInfo(typeof(BWChaos.Chaos), BWChaos.BuildInfo.Name, BWChaos.BuildInfo.Version, BWChaos.BuildInfo.Author, BWChaos.BuildInfo.DownloadLink)]
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]
[assembly: VerifyLoaderVersion(0, 5, 1, true)]