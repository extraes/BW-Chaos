using Il2CppInterop.Runtime;
using System.Diagnostics;

namespace BLChaos.Effects;

internal class BouncyHouse : EffectBase
{
    public BouncyHouse() : base("Bouncy House", 90) { }
    Dictionary<Collider, PhysicMaterial> originalMaterials = new Dictionary<Collider, PhysicMaterial>();
    static PhysicMaterial pMat;
    //[EffectPreference("Bouncifies everything instead of just the player")] static bool bouncifyAll = false;
    // this shit is so stupid bruh if anyone makes this over 1 its gonna be a fucking perpetual and exponential motion machine
    [RangePreference(0, 10f, 0.25f)] public static float mult = 1;
    List<Bouncifier> bouncifiers = new();

    public override void OnEffectStart()
    {
        if (pMat == null)
        {
            pMat = new PhysicMaterial
            {
                hideFlags = HideFlags.DontUnloadUnusedAsset,
                bounciness = 1000,
                bounceCombine = PhysicMaterialCombine.Maximum,
                dynamicFriction = 100,
                staticFriction = 100,
                frictionCombine = PhysicMaterialCombine.Maximum,
            };
        }
    }
    
    public override void OnEffectEnd()
    {
        foreach (var colMat in originalMaterials)
        {
            if (colMat.Key == null) continue;
            colMat.Key.material = colMat.Value;
        }

        foreach (Bouncifier bouncifier in bouncifiers)
        {
            if (bouncifier == null) continue;
            
            GameObject.Destroy(bouncifier);
        }
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        Stopwatch sw = Stopwatch.StartNew();

        var cols = GameObject.FindObjectsOfType<Collider>();
        Log($"Changing {cols.Length} collider materials");
        foreach (Collider col in cols)
        {
            if (sw.ElapsedMilliseconds > 3) // only take up 3ms of frame time.
            {
                Log("Waiting a frame after taking 3ms on current frame for collider materials!");
                yield return null;
                sw.Restart();
            }

            originalMaterials[col] = col.sharedMaterial;
            col.material = pMat;
        }

        yield return null;

        //var rbs = GameObject.FindObjectsOfType<Rigidbody>();
        //var rbs = bouncifyAll ? GameObject.FindObjectsOfType<Rigidbody>() : GlobalVariables.Player_RigManager.GetComponentsInChildren<Rigidbody>();
        var rbs = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<Rigidbody>()).Select(o => o.TryCast<Rigidbody>()).ToArray();
        Log($"Adding bouncifier to {rbs.Length} rigidbodies");
        foreach (Rigidbody? rb in rbs)
        {
            if (rb == null)
                continue;

            if (sw.ElapsedMilliseconds > 3) // only take up 3ms of frame time.
            {
                Log("Waiting a frame after taking 3ms on current frame!");
                yield return null;
                sw.Restart();
            }

            if (Bouncifier.gameobjectsWithBouncifiers.Contains(rb.gameObject.GetInstanceID()))
            {
                Bouncifier bouncer = rb.gameObject.AddComponent<Bouncifier>();
                bouncer.rb = rb;
                bouncifiers.Add(bouncer);
            }
        }
    }
}

[RegisterTypeInIl2Cpp]
internal class Bouncifier : MonoBehaviour
{
    public Bouncifier(IntPtr ptr) : base(ptr) { }
    public static readonly HashSet<int> gameobjectsWithBouncifiers = new();

    public Rigidbody rb; // set by CoRun

    public void Awake() => gameobjectsWithBouncifiers.Add(gameObject.GetInstanceID());
    public void OnDestroy() => gameobjectsWithBouncifiers.Remove(gameObject.GetInstanceID());

    public void OnCollisionEnter(Collision c)
    {
        rb.AddForce(c.impulse * BouncyHouse.mult);
    }
}