using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class DistantSound : EffectBase
{
    public DistantSound() : base("Distant Sounds", 90) { }
    [RangePreference(0, 45, 1)]
    static float minWaitTime = 5;
    [RangePreference(0, 45, 1)]
    static float variationWaitTime = 15;
    [RangePreference(0, 45, 1)]
    static float minPlayPos = 15;
    [RangePreference(0, 100, 1)]
    static float variationPlayPos = 50;
    [RangePreference(0, 1, 0.05f)]
    static float volume = 0.05f;

    //AudioSource aSource;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>().Where(c => c.length > 0.1f).ToArray();
        yield return null;

        while(Active)
        {
            Vector3 playPosOffset = Random.onUnitSphere * (minPlayPos + Random.value * variationPlayPos);
            AudioClip clip = clips.Random();
            AudioSource.PlayClipAtPoint(clip, GlobalVariables.Player_PhysBody.rbHead.transform.position + playPosOffset, volume);

            yield return new WaitForSeconds(clip.length);
            yield return new WaitForSeconds(minWaitTime + Random.value * variationWaitTime);
        }
    }
}
