#if !NOBONELIB
using Il2CppSLZ.Marrow.PuppetMasta;

namespace BLChaos.Effects;

internal class Fold : EffectBase
{
    public Fold() : base("<b>Fold.</b>") { }

    public override void OnEffectStart()
    {
        try
        {
            foreach (PuppetMaster pm in GameObject.FindObjectsOfType<PuppetMaster>())
            {
                pm.Kill();
            }
        }
        catch { }
    }

}
#endif