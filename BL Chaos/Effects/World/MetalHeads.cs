using Il2CppInterop.Runtime;
using Il2CppSLZ.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;

namespace BLChaos.Effects;

internal class MetalHeads : EffectBase
{
    static MetalHeads()
    {
        Instances<ImpactProperties>.TryAutoCache();
    }

    static readonly string[] metallicWords =
    {
        "metal",
        "steel",
        "iron",
        "hydraulic",
        "gun",
        "lead",
    };

    [RangePreference(1, 10, 1)] public static int framesToWait = 4;
    [RangePreference(0.125f, 10f, 0.125f)] public static float forceMultiplier = 0.5f;
    public MetalHeads() : base("Metal heads", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }

    private readonly List<MetalHeadBehavior> trackedBehaviours = new List<MetalHeadBehavior>();
    public override void OnEffectStart()
    {
        MelonCoroutines.Start(ApplyMonoBehaviour());
    }
    public override void OnEffectEnd()
    {
        foreach (UnityEngine.Object comp in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<MetalHeadBehavior>())) GameObject.Destroy(comp);
    }

    private IEnumerator ApplyMonoBehaviour()
    {
        bool stagger = false;
        MetalHeadBehavior.target = GlobalVariables.Player_PhysRig.torso.rbHead.transform;
        foreach (MarrowBody mb in GameObject.FindObjectsOfType<MarrowBody>())
        {
            bool isInHands = mb.transform.IsChildOf(Player.LeftHand.transform) || mb.transform.IsChildOf(Player.RightHand.transform);
            if (mb.transform.IsChildOfRigManager() && !isInHands)
                continue;

            //todo: strcmp is slower than like... caching?
            if (Instances<ImpactProperties>.TryGetFromCache(mb.gameObject, out var ip) && !metallicWords.Any(m => ip.name.Contains(m, StringComparison.InvariantCultureIgnoreCase)))
                continue;

            // we dont want to mess with things that already have joints, are in the list, or are static
            GameObject go = mb.gameObject; //                V luckily passing null to contains doesnt error out
            if (go.GetComponent<MetalHeadBehavior>()) continue;
#if DEBUG
            //
            //
            //($"Gave {go.name} the script");
#endif

            trackedBehaviours.Add(go.AddComponent<MetalHeadBehavior>());

            if (stagger = !stagger) yield return new WaitForFixedUpdate();

        }

    }
}

[RegisterTypeInIl2Cpp]
public class MetalHeadBehavior : MonoBehaviour
{
    public MetalHeadBehavior(IntPtr ptr) : base(ptr) { }

    private static float Mult => MetalHeads.forceMultiplier;
    private static int FramesToWait => MetalHeads.framesToWait;
    public static Transform target;
    private bool isNear = false;
    float lastUpdate = Time.time;
    private Rigidbody rb;
    private object CToken;
    public void OnEnable()
    {
        rb = GetComponent<MarrowBody>()._rigidbody;
        CToken = MelonCoroutines.Start(CheckDist());
    }

    // shoutouts to camobiwon for suggesting i use a pd controller (and sending link)
    public void FixedUpdate()
    {
        if (!isNear || (Time.frameCount % FramesToWait != 0)) return;

        // https://digitalopus.ca/site/pd-controllers/ lol
        float dt = Time.time - lastUpdate;
        dt = dt == 0 ? Time.fixedDeltaTime * FramesToWait : dt;
        Vector3 p = transform.position; //our current position
        Vector3 v = rb.velocity; //our current velocity
        Vector3 force = rb.mass * (target.position - p - v * dt) / (dt);

        rb.AddForce(Vector3.ClampMagnitude(force * Mult, 200 * rb.mass));
        lastUpdate = Time.time;
    }

    public void Destroy()
    {
        MelonCoroutines.Stop(CToken);
    }

    private IEnumerator CheckDist()
    {
        while (true)
        {
            try
            {
                // null-check this because MelonCoroutines dont stop with a gameobject
                if (this == null || gameObject == null || !gameObject.active) yield break;
                // dont do shit if we're not in 15m, and also dont do shit if we're being held by the player (or otherwise a part of the player)
                isNear = ((target.position - gameObject.transform.position).sqrMagnitude < 7 * 7) && !transform.IsChildOfRigManager();
            }
            catch { isNear = false; }
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }
}

