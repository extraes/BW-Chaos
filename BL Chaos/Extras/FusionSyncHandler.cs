using System.Reflection;


// Shameless copy paste from ZCubed's example
namespace BLChaos.Extras;

static class FusionSyncHandler
{
    public static bool isVersionMismatch = false;
    static Assembly? moduleAsm;

    // Loads a module using only pure reflection
    internal static void Init()
    {
        // First, check if Fusion is loaded
        Assembly? entanglementAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.GetName().Name == "LabFusion");

        if (entanglementAssembly == null)
        {
            Chaos.Warn("Fusion wasn't found! Is it installed?");
            Chaos.Warn("Bailing on loading Fusion networking module.");
            return;
        }
        
        //// Then get the ModuleHandler dynamically
        //Type? moduleHandlerType = entanglementAssembly.GetType("LabFusion.SDK.Modules.ModuleHandler.LoadModule");

        //if (moduleHandlerType == null) throw new NullReferenceException("Failed to find ModuleHandler");

        //// Then try to get SetupModule()
        //MethodInfo? setupModuleMethod = moduleHandlerType.GetMethod("SetupModule", BindingFlags.Static | BindingFlags.Public);

        //if (setupModuleMethod == null) throw new MissingMethodException("Failed to find SetupModule()");

        // Then load our embedded module
        // Layout for resources is basically YOUR_ASSEMBLY.FOLDER.FILE.EXTENSION
        byte[] moduleRaw = null;

        Chaos.Assembly.UseEmbeddedResource("BLChaos.Resources.FusionSync.dll", b => moduleRaw = b);
        moduleAsm = Assembly.Load(moduleRaw!);

        // Load it into the appdomain
        Chaos.Log("Syncing is enabled and Fusion was found! So far so good! Now we give Fusion our handler!");
        
        //calling setup isnt necessary, Fusion will automatically register the module when its loaded.
        // Then call the setup method reflectively
        //setupModuleMethod.Invoke(null, new object[] { moduleAssembly });
    }
}
