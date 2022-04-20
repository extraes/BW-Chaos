using MelonLoader;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects;

internal class RandomTimeScale : EffectBase
{
    public RandomTimeScale() : base("Random slowmo", 90) { }

    public override void OnEffectStart()
    {
        Utilities.DisableSloMo();
    }
    public override void OnEffectEnd()
    {
        Time.timeScale = 1;
        Utilities.EnableSloMo();
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

            SendNetworkData(waitTime + "," + timeScale);

            yield return new WaitForSecondsRealtime(waitTime);
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1;

        }
    }

    public override void HandleNetworkMessage(string data)
    {
        string[] datas = data.Split(',');
        float f1 = float.Parse(datas[0]);
        float f2 = float.Parse(datas[1]);
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
