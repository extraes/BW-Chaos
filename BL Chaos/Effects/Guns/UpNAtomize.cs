using Jevil.Patching;
using MelonLoader;

namespace BLChaos.Effects;

internal class UpNAtomize : EffectBase
{
    public UpNAtomize() : base("Up-N-Atomize", 90, EffectTypes.LAGGY) { }
    [RangePreference(0, 10, 0.25f)] static float radius = 2f;
    [RangePreference(0, 50, 0.5f)] static float forceMultiplier = 1f;
    [RangePreference(1, 10, 1)] static int rbsPerFrame = 4;

    public override void OnEffectStart()
    {
        GameCallbacks.OnBulletHit += OnBulletHit;
    }

    private void OnBulletHit(Collider col, Vector3 pos, Vector3 normal)
    {
#if DEBUG
        Chaos.Log("Bullet collided with " + col.name);
        Chaos.Log("Vector3 argument 2 = " + pos.ToString());
        Chaos.Log("Vector3 argument 3 = " + normal.ToString());
#endif

        Collider[] cols = Physics.OverlapSphere(pos, radius * 1.25f);
        MelonCoroutines.Start(ApplyForces(cols.Take(cols.Length / 2), pos));
        MelonCoroutines.Start(ApplyForces(cols.Skip(cols.Length / 2), pos));
    }

    private IEnumerator ApplyForces(IEnumerable<Collider> cols, Vector3 origin)
    {
        yield return null;
        int counter = 0;
        foreach (Collider col in cols)
        {
            if (col == null || col.attachedRigidbody == null || col.attachedRigidbody.isKinematic) continue;
            Rigidbody rb = col.attachedRigidbody;
            rb.AddExplosionForce(rb.mass * forceMultiplier * 25, origin, radius, 2);
            if (counter++ % rbsPerFrame == 0) yield return null;
        }
    }
}
