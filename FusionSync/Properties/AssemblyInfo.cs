
using BLChaos.Sync;
using System.Reflection;
using System.Runtime.Versioning;

[assembly: AssemblyTitle(BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct(BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BuildInfo.Author)]

[assembly: LabFusion.SDK.Modules.ModuleInfo(typeof(ChaosModule), BuildInfo.Name, BuildInfo.Version, BuildInfo.Author, BuildInfo.Abbreviation, BuildInfo.AutoRegister, BuildInfo.Color)]

[assembly: RequiresPreviewFeatures]