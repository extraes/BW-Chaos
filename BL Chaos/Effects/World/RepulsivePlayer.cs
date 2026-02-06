using Il2CppInterop.Runtime;
using MelonLoader;

namespace BLChaos.Effects;

internal class RepulsivePlayer : EffectBase
{
    [RangePreference(1, 10, 1)] public static int framesToWait = 4;
    [RangePreference(0.125f, 10f, 0.125f)] public static float forceMultiplier = 0.5f;
    public RepulsivePlayer() : base("Repulsive player", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }

    private readonly List<RepulseBehaviour> addedComponents = new List<RepulseBehaviour>();
    public override void OnEffectStart()
    {
        MelonCoroutines.Start(ApplyMonoBehaviour());
    }
    public override void OnEffectEnd()
    {
        foreach (UnityEngine.Object comp in GameObject.FindObjectsOfTypeAll(Il2CppType.Of<RepulseBehaviour>())) GameObject.Destroy(comp);
    }

    private IEnumerator ApplyMonoBehaviour()
    {
        bool stagger = false;
        RepulseBehaviour.target = GlobalVariables.Player_PhysRig.rbFeet.transform;
        foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
        {
            bool isInHands = rb.transform.IsChildOf(Player.LeftHand.transform) || rb.transform.IsChildOf(Player.RightHand.transform);
            if (rb.transform.IsChildOfRigManager() && !isInHands)
                continue;

            // we dont want to mess with things that already have joints, are in the list, or are static
            GameObject go = rb.gameObject; //                V luckily passing null to contains doesnt error out
            if (go.GetComponent<RepulseBehaviour>()) continue;
#if DEBUG
            //Chaos.Log($"Gave {go.name} the script");
#endif

            RepulseBehaviour repulsor = go.AddComponent<RepulseBehaviour>(); // omg irony man referenc
            repulsor.rb = rb;
            addedComponents.Add(repulsor);

            if (stagger = !stagger) yield return new WaitForFixedUpdate();

        }

    }
}

[RegisterTypeInIl2Cpp]
public class RepulseBehaviour : MonoBehaviour
{
    public RepulseBehaviour(IntPtr ptr) : base(ptr) { }

    private static float Mult => RepulsivePlayer.forceMultiplier;
    //                         optuhmuhzayshun V
    private static int FramesToWait => RepulsivePlayer.framesToWait;
    public static Transform target;
    private bool isNear = false;
    float lastUpdate = Time.time;
    public Rigidbody rb;
    private object CToken;
    public void OnEnable()
    {
        target = GlobalVariables.Player_PhysRig.rbFeet.transform;
        if (rb == null) rb = GetComponent<Rigidbody>();
        CToken = MelonCoroutines.Start(CheckDist());
    }

    // shoutouts to camobiwon for suggesting i use a pd controller (and sending link)
    public void FixedUpdate()
    {
        if (!isNear || (Time.frameCount % FramesToWait != 0) || rb == null) return;

        // https://digitalopus.ca/site/pd-controllers/ lol
        float dt = Time.time - lastUpdate;
        dt = dt == 0 ? Time.fixedDeltaTime * FramesToWait : dt;
        Vector3 p = transform.position; //our current position
        Vector3 v = rb.velocity; //our current velocity
        // subt V3.up because then rb's wont try to go into the floor   V
        Vector3 force = rb.mass * (target.position - Vector3.up - p - v * dt) / (dt);
        
        rb.AddForce(-Vector3.ClampMagnitude(force * Mult, 100 * rb.mass));
        lastUpdate = Time.time;
    }

    public void Destroy()
    {
        MelonCoroutines.Stop(CToken); // if this nulls, oh well, this thread is dying anyway
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

