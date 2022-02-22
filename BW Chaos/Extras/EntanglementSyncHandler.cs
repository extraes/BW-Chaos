using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


// Shameless copy paste from ZCubed's example
namespace BWChaos.Extras
{
    static class EntanglementSyncHandler
    {
        public static bool isVersionMismatch = false;

        // Loads a module using only pure reflection
        internal static void Init()
        {
            // First, check if Entanglement is loaded
            Assembly entanglementAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.GetName().Name == "Entanglement");

            if (entanglementAssembly == null)
            {
                try { Utilities.SpawnAd("Entanglement wasn't found! Is it installed?"); } catch { }
                throw new DllNotFoundException("Couldn't find Entanglement! Did you mean to set syncEffectsViaEntanglement to false?");
            }

            // Then get the ModuleHandler dynamically
            Type moduleHandlerType = entanglementAssembly.GetType("Entanglement.Modularity.ModuleHandler");

            if (moduleHandlerType == null) throw new NullReferenceException("Failed to find ModuleHandler");

            // Then try to get SetupModule()
            MethodInfo setupModuleMethod = moduleHandlerType.GetMethod("SetupModule", BindingFlags.Static | BindingFlags.Public);

            if (setupModuleMethod == null) throw new MissingMethodException("Failed to find SetupModule()");

            // Then load our embedded module
            // Layout for resources is basically YOUR_ASSEMBLY.FOLDER.FILE.EXTENSION
            byte[] moduleRaw = null;
            Assembly moduleAssembly;

            Chaos.Assembly.UseEmbeddedResource("BWChaos.Resources.EntanglementSync.dll", b => moduleRaw = b);
            moduleAssembly = Assembly.Load(moduleRaw);

            // Load it into the appdomain
            Chaos.Log("Syncing is enabled and Entanglement was found! So far so good! Now we give Entanglement our handler!");
            // Then call the setup method reflectively
            setupModuleMethod.Invoke(null, new object[] { moduleAssembly });
        }
    }
}
