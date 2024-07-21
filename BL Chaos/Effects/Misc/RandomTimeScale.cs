using Jevil.Patching;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class RandomTimeScale : EffectBase
{
    public RandomTimeScale() : base("Random slowmo", 90) { }
    static bool active;

    static RandomTimeScale()
    {
        Disable.When(() => active, typeof(Control_GlobalTime).GetMethod(nameof(Control_GlobalTime.DECREASE_TIMESCALE)));
    }

    public override void OnEffectStart()
    {
        active = true;
    }

    public override void OnEffectEnd()
    {
        Time.timeScale = 1;
        active = false;
    }

    [AutoCoroutine]
    public IEnumerator ChangeTime()
    {
        float[] times = new float[] { 0.125f, 0.25f, 0.5f };
        yield return null;

        while (Active)
        {
            float waitTime = Random.RandomRange(6, 10);
            float timeScale = times.Random();
            byte[] data = new byte[sizeof(float) * 2];

            BitConverter.GetBytes(waitTime).CopyTo(data, 0);
            BitConverter.GetBytes(timeScale).CopyTo(data, sizeof(float));
            SendNetworkData(data);

            yield return new WaitForSecondsRealtime(waitTime);
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1;

        }
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        float f1 = BitConverter.ToSingle(data, 0);
        float f2 = BitConverter.ToSingle(data, sizeof(float));
        MelonCoroutines.Start(NetScale(f1, f2));
    }

    private IEnumerator NetScale(float waitTime, float timeScale)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
    }
}
