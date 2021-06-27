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
[assembly: AssemblyVersion(BW_Chaos.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BW_Chaos.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]

[assembly: MelonInfo(typeof(BW_Chaos.BW_Chaos), BW_Chaos.BuildInfo.Name, BW_Chaos.BuildInfo.Version, BW_Chaos.BuildInfo.Author, BW_Chaos.BuildInfo.DownloadLink)]
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]