using MelonLoader;
using System.Diagnostics;

namespace BLChaos.Effects;

internal class FartWithReverb : EffectBase
{
    public FartWithReverb() : base("Fart With Reverb", 5, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }
    [RangePreference(0.25f, 10, 0.25f)] static float forceMultiplier = 2f;

    private static Transform target;
    private static AudioClip clip;
    public override void OnEffectStart()
    {
        clip = clip == null ? GlobalVariables.EffectResources.LoadAsset("assets/sounds/fart with extra reverb.mp3").Cast<AudioClip>() : clip;
        target = GlobalVariables.Player_PhysRig.rbFeet.transform;

        List<Rigidbody> rbs = GameObject.FindObjectsOfType<Rigidbody>().ToList();
        // match CPU count, not thread count, and higher numbers lag spike the game longer
        int perCPUCount = 2 * rbs.Count / 8;
#if DEBUG
        Stopwatch sw = Stopwatch.StartNew();
#endif

        foreach (var rbPart in rbs.SplitList(perCPUCount + 1))
        {      
            MelonCoroutines.Start(ApplyForces(rbPart));
#if DEBUG
            Log($"Started {nameof(ApplyForces)} with {rbPart.Count()} rigidbodies");
#endif
        }
#if DEBUG
        sw.Stop();
        Log("Finished starting coroutines in " + sw.ElapsedMilliseconds + "ms");
#endif

        GlobalVariables.SFXPlayer.PlayClip(clip, 0.9f);
    }

    private IEnumerator ApplyForces(IEnumerable<Rigidbody> rbs)
    {
        yield return null;
        bool stagger = false;
        foreach (Rigidbody rb in rbs)
        {
            if (!Active || rb is null) yield break;

            // ignore it if its far away
            if (Vector3.Distance(target.position, rb.transform.position) > 30) continue;

            ////float dt = Time.fixedDeltaTime;
            //Vector3 p = rb.transform.position; //our current position
            //Vector3 v = rb.velocity; //our current velocity
            //// subt V3.up because then rb's wont try to go into the floor   V
            //Vector3 force = rb.mass * (target.transform.position - p) * 100;

            //rb.AddForce(-Vector3.ClampMagnitude(force * forceMultiplier, 500 * rb.mass));
            rb.AddExplosionForce(forceMultiplier * 2, target.position, 30, 5, ForceMode.VelocityChange);

            if (stagger = !stagger) yield return new WaitForFixedUpdate();
        }

    }
}

