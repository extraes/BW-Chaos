using Il2CppSLZ.Interaction;
using Jevil.Patching;
using MelonLoader;

namespace BLChaos.Effects;

internal class SlowmoPunch : EffectBase
{
    public SlowmoPunch() : base("SlowMo Punch", 60) { }
    [RangePreference(0, 5f, 0.05f)] static float returnTime = 1f;
    float YieldTime => returnTime / 20;

    public override void OnEffectStart() => onPunch += RunSlowmo;

    public override void OnEffectEnd() => onPunch -= RunSlowmo;

    public void RunSlowmo() => MelonCoroutines.Start(Slowmo_OnPunch());

    static Action? onPunch;

    private IEnumerator Slowmo_OnPunch()
    {
#if DEBUG
        Log($"Started {nameof(Slowmo_OnPunch)}, waiting {YieldTime}s 20 times until {returnTime} passes and time hits 1 again");
#endif
        Time.timeScale = 0.05f;

        while (Time.timeScale < 1f && Active)
        {
            Time.timeScale += 0.05f;
            yield return new WaitForSecondsRealtime(YieldTime);
        }
        Time.timeScale = 1;
    }
}
