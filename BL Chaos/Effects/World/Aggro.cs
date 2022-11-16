using SLZ.AI;
using UnityEngine;

namespace BLChaos.Effects;

internal class Aggro : EffectBase
{
    public Aggro() : base("Aggro", EffectTypes.DONT_SYNC) { }
    static TriggerRefProxy trp;

    public override void OnEffectStart()
    {
        trp = trp != null ? trp : GameObject.Find("PlayerTrigger").GetComponent<TriggerRefProxy>();
        // try catch because despite the ?. it still nullrefs. idk why
        try
        {
            GameObject.FindObjectsOfType<AIBrain>().ForEach(b => b?.behaviour?.SetAgro(trp));
        }
        catch { }
    }
}
