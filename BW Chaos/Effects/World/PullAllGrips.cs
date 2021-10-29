using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using System.Collections;
using UnityEngine;

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

            foreach (ForcePullGrip grip in GameObject.FindObjectsOfType<ForcePullGrip>())
            {
                grip.Pull(Utilities.GetRandomPlayerHand());
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }
}
