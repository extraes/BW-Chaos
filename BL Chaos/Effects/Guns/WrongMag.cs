using BoneLib;
using BoneLib.Nullables;
using HarmonyLib;
using Jevil;
using SLZ.Interaction;
using SLZ.Marrow.Pool;
using SLZ.Props.Weapons;
using System;
using System.Linq;
using UnityEngine;

namespace BLChaos.Effects;

internal class WrongMag : EffectBase
{
    public WrongMag() : base("Wrong Mag", 60) { }
    static Magazine[] mags = Utilities.FindAll<Magazine>().ToArray();
    static Action<Hand> magGrabbed;

    [HarmonyPatch(typeof(InventoryAmmoReceiver), nameof(InventoryAmmoReceiver.OnHandGrab))]
    static class AmmoPouchPatch
    {
        public static void Postfix(Hand hand)
        {
            magGrabbed?.Invoke(hand);
        }
    }

    public override void OnEffectStart()
    {
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

    private static async void ChangeMag(Hand hand)
    {
        // stolen from MTINM
        // thx 4 open sauce, chap
        Magazine mag = mags.Random();
        AssetPoolee magObject = await NullableMethodExtensions.PoolManager_SpawnAsync(mag.magazineState.magazineData.spawnable).ToTask();
        Grip grip = magObject.GetComponent<Grip>();
        //magObject.transform.rotation = grip.transform.transform(hand).rotation;
        Vector3 localTarget = (grip.targetTransform != null) ? grip.targetTransform.localPosition : Vector3.zero;
        magObject.transform.position = hand.palmPositionTransform.position - localTarget;
        Magazine currentMagInHand = Player.GetComponentInHand<Magazine>(hand);
        //currentMagInHand.interactableHost.Drop();
        hand.DetachObject();
        hand.DetachJoint(true, null);
        currentMagInHand.gameObject.Destroy();
        grip.Snatch(hand, true);
    }
}
