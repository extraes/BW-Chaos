using MelonLoader;
using StressLevelZero.Interaction;
using System.Collections;
using UnityEngine;
using ModThatIsNotMod;

namespace BWChaos.Effects
{
    internal class PullAllGrips : EffectBase
    {
        public PullAllGrips() : base("Force pull all grips", 30) { }

        private object CToken = null;
        public override void OnEffectStart() => CToken = MelonCoroutines.Start(ActivateGrips());
        public override void OnEffectEnd()
        {
            MelonCoroutines.Stop(CToken);
            foreach (var grip in GameObject.FindObjectsOfType<ForcePullGrip>())
            {
                grip?.CancelPull(Player.leftHand);
                grip?.CancelPull(Player.rightHand);
            }
        }

        public IEnumerator ActivateGrips()
        {
            yield return null;
            ForcePullGrip[] grips = GameObject.FindObjectsOfType<ForcePullGrip>();
            while (Active)
            {
                foreach (ForcePullGrip grip in grips)
                {
                    grip.Pull(Utilities.GetRandomPlayerHand());
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
        }

    }
}
