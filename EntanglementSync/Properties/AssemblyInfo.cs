using Entanglement.Modularity;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EntanglementSync")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("EntanglementSync")]
[assembly: AssemblyCopyright("Copyright © extraes 2021")] // no rights reserved (is this comment legally binding?)
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8c782def-eca1-44e6-a1e9-cb9cc6a5ea22")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


// Inside AssemblyInfo.cs we'd put the following to initialize this class on load
// [assembly: EntanglementModuleInfo(typeof(ExampleModule), "Example Module", "0.0.0", "Nobody", "Example")]
// Arguments:
// 1 = the module type to initialize
// 2 = the name of the module
// 3 = the string representation of the version
// 4 = the author
// 5 = the abbreviation, basically what it's logged as (optional)

[assembly: EntanglementModuleInfo(typeof(BLChaos.Sync.ChaosSyncHandler), "BWChaos", "2.2.1", "extraes", "Chaos")] 