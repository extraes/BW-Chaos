using UnityEngine;

namespace BWChaos.Effects;

internal class FreezeAll : EffectBase
{
    public FreezeAll() : base("Freeze Everything") { }

    public override void OnEffectStart()
    {
        try
        {
            foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
            {
                if (rb == null || rb.transform.IsChildOfRigManager()) continue;
                rb?.Sleep();
            }
        }
        catch { }
    }
}
