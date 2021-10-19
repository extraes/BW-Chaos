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
                Utilities.SpawnAd("Entanglement wasn't found! Is it installed?");
                throw new DllNotFoundException("Couldn't find Entanglement!");
            }

            // Then get the ModuleHandler dynamically
            Type moduleHandlerType = entanglementAssembly.GetType("Entanglement.Modularity.ModuleHandler");

            if (moduleHandlerType == null) throw new NullReferenceException("Failed to find ModuleHandler");

            // Then try to get SetupModule()
            MethodInfo setupModuleMethod = moduleHandlerType.GetMethod("SetupModule", BindingFlags.Static | BindingFlags.Public);

            if (setupModuleMethod == null) throw new MissingMethodException("Failed to find SetupModule()");

            // Then load our embedded module
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            string[] manifestResources = thisAssembly.GetManifestResourceNames();

            // Layout for resources is basically YOUR_ASSEMBLY.FOLDER.FILE.EXTENSION
            byte[] moduleRaw = null;

            using (Stream str = thisAssembly.GetManifestResourceStream("BWChaos.Resources.EntanglementSync.dll"))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                str.CopyTo(memoryStream);
                moduleRaw = memoryStream.ToArray();
            }

            // Load it into the appdomain
            Assembly moduleAssembly = Assembly.Load(moduleRaw);
            MelonLogger.Msg("Syncing is enabled and Entanglement was found! So far so good! Now we give Entanglement our handler!");
            // Then call the setup method reflectively
            setupModuleMethod.Invoke(null, new object[] { moduleAssembly });
        }
    }
}
