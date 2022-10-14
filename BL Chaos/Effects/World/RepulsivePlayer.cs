using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BLChaos.Effects;

internal class RepulsivePlayer : EffectBase
{
    [RangePreference(1, 10, 1)] public static int framesToWait = 4;
    [RangePreference(0.125f, 10f, 0.125f)] public static float forceMultiplier = 0.5f;
    public RepulsivePlayer() : base("Repulsive player", 30, EffectTypes.LAGGY | EffectTypes.DONT_SYNC) { }

    private readonly List<RepulseBehaviour> gameObjects = new List<RepulseBehaviour>();
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
        foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
        {
            // we dont want to mess with things that already have joints, are in the list, or are static
            GameObject go = rb.gameObject; //                V luckily passing null to contains doesnt error out
            if (gameObjects.Contains(go.GetComponent<RepulseBehaviour>())) continue;
#if DEBUG
            //Chaos.Log($"Gave {go.name} the script");
#endif

            gameObjects.Add(go.AddComponent<RepulseBehaviour>());

            if (stagger = !stagger) yield return new WaitForFixedUpdate();

        }

    }
}

[RegisterTypeInIl2Cpp]
public class RepulseBehaviour : MonoBehaviour
{
    public RepulseBehaviour(IntPtr ptr) : base(ptr) { }

    private static readonly float mult = RepulsivePlayer.forceMultiplier;
    //                         optuhmuhzayshun V
    private static readonly int framesToWait = RepulsivePlayer.framesToWait;
    private static Transform target;
    private bool isNear = false;
    private Rigidbody rb;
    private object CToken;
    public void OnEnable()
    {
        target = GlobalVariables.Player_PhysBody.transform;
        rb = GetComponent<Rigidbody>();
        CToken = MelonCoroutines.Start(CheckDist());
    }

    // shoutouts to camobiwon for suggesting i use a pd controller (and sending link)
    public void FixedUpdate()
    {
        if (!isNear || (Time.frameCount % framesToWait != 0) || rb == null) return;

        // https://digitalopus.ca/site/pd-controllers/ lol
        float dt = Time.fixedDeltaTime;
        Vector3 p = transform.position; //our current position
        Vector3 v = rb.velocity; //our current velocity
        // subt V3.up because then rb's wont try to go into the floor   V
        Vector3 force = rb.mass * (target.transform.position - Vector3.up - p - v * dt) / (dt);

        rb.AddForce(-Vector3.ClampMagnitude(force * mult, 100 * rb.mass));
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
                if (this?.gameObject == null || !gameObject.active) yield break;
                // dont do shit if we're not in 15m, and also dont do shit if we're being held by the player (or otherwise a part of the player)
                isNear = ((target.position - gameObject.transform.position).sqrMagnitude < 7 * 7) && !transform.IsChildOfRigManager();
            }
            catch { isNear = false; }
            yield return new WaitForSecondsRealtime(1);
        }
    }
}

