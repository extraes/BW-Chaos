#if !NOBONELIB
#endif

namespace BLChaos.Effects;

internal class FuckYourMag : EffectBase
{
    public FuckYourMag() : base("Fuck Your Magazine", 90, EffectTypes.HIDDEN) { }
    [RangePreference(1, 10, 1)] static float minWaitTime = 5;
    [RangePreference(10, 20, 1)] static float maxWaitTime = 10f;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        yield return null;
        while (Active)
        {
#if NOBONELIB
            foreach (Gun gun in GameObject.FindObjectsOfType<Gun>())
            {
                Utilities.Try(gun.EjectCartridge);
            }
#else
            Gun gun = Player.GetComponentInHand<Gun>(Utilities.GetRandomPlayerHand());

            gun?.EjectCartridge(); //todo: test (old code = .magazineSocket?.MagazineRelease(); )
#endif

            yield return new WaitForSecondsRealtime(Random.RandomRange(minWaitTime, maxWaitTime));
        }
    }
}
