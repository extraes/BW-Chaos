#if !NOBONELIB
using Jevil.Patching;

namespace BLChaos.Effects;

internal class WrongMag : EffectBase
{
    public WrongMag() : base("Wrong Mag", 60) { }
    static Magazine[] mags = Utilities.FindAll<Magazine>().ToArray();

    public override void OnEffectStart()
    {
        if (mags == null || mags.Length == 0 || mags[0] == null)
        {
            mags = Utilities.FindAll<Magazine>().ToArray();
        }

        GameCallbacks.OnMagGrabbed += ChangeMag;
    }

    public override void OnEffectEnd() => GameCallbacks.OnMagGrabbed -= ChangeMag;

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
        Poolee magObject = await AssetSpawner.SpawnAsync(mag.magazineState.magazineData.spawnable, Vector3.zero);
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
#endif