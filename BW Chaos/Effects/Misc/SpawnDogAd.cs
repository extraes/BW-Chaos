using UnityEngine;
using System.Collections;

namespace BWChaos.Effects
{
    internal class SpawnDogAd : EffectBase
    {
        public SpawnDogAd() : base("Spawn Dog Ad", 75) { }

        public override void OnEffectStart() => MelonLoader.MelonCoroutines.Start(CoRun());

        private IEnumerator CoRun()
        {
            while (Active)
            {
                // todo: stress test this
                ModThatIsNotMod.RandomShit.AdManager.CreateDogAd();
                yield return new WaitForSecondsRealtime(2.5f);
            }
        }
    }
}
