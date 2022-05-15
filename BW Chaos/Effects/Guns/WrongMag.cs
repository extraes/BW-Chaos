using HarmonyLib;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using System;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects;

internal class WrongMag : EffectBase
{
    public WrongMag() : base("Wrong Mag", 60) { }
    static Magazine[] mags = Utilities.FindAll<Magazine>().ToArray();
    static Action<Hand> magGrabbed;
    AmmoPouch ammoPouch;

    [HarmonyPatch(typeof(AmmoPouch), nameof(AmmoPouch.OnSpawnGrab))]
    static class AmmoPouchPatch
    {
        public static void Postfix(Hand hand)
        {
            magGrabbed?.Invoke(hand);
        }
    }

    public override void OnEffectStart()
    {
        ammoPouch = GameObject.FindObjectOfType<AmmoPouch>();

        if (mags == null || mags.Length == 0 || mags[0] == null)
        {
            mags = Utilities.FindAll<Magazine>().ToArray();
        }

        magGrabbed += ChangeMag;
    }

    public override void OnEffectEnd() => magGrabbed -= ChangeMag;

    //[AutoCoroutine]
    //public IEnumerator CoRun()
    //{
    //    yield return null;
    //    if (isNetworked) yield break;
    //    while (Active)
    //    {
    //        ammoPouch.UpdateArt(weights.Random());
    //        ammoPouch.SwitchMagazine(platforms.Random());
    //        yield return new WaitForSecondsRealtime(1f);
    //    }
    //}

    private static void ChangeMag(Hand hand)
    {
        // stolen from MTINM
        // thx 4 open sauce, chap
        Magazine mag = mags.Random();
        GameObject magObject = GameObject.Instantiate(mag.magazineData.spawnableObject.prefab);
        Grip grip = magObject.GetComponent<Grip>();
        magObject.transform.rotation = grip.GetRotatedGripTargetTransformWorld(hand).rotation;
        Vector3 localTarget = (grip.targetTransform != null) ? grip.targetTransform.localPosition : Vector3.zero;
        magObject.transform.position = hand.palmPositionTransform.position - localTarget;
        Magazine currentMagInHand = Player.GetComponentInHand<Magazine>(hand);
        currentMagInHand.interactableHost.Drop();
        hand.DetachObject(currentMagInHand.gameObject, true);
        hand.DetachJoint(true, null);
        currentMagInHand.gameObject.Destroy();
        grip.Snatch(hand, true);
    }
}
