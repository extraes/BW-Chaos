using Entanglement.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLChaos.Sync
{
    internal class PlayerRepresenting : Effects.EffectBase
    {
        public PlayerRepresenting() : base("The Entities...", 60) { }

        readonly List<PlayerRepresentation> reps = new List<PlayerRepresentation>(16);
        public override void HandleNetworkMessage(byte[][] data)
        {
            int idx = data[1][0];
            Transform transform = reps[idx].repTransforms[0].root.transform;
            transform.DeserializePosRot(data[0]);
        }

        public override void OnEffectStart()
        {
            for (int i = 0; i < reps.Capacity; i++)
            {
                reps.Add(new PlayerRepresentation("Him", i));
            }
        }

        public override void OnEffectEnd()
        {
            foreach (PlayerRepresentation rep in reps)
            {
                GameObject.Destroy(rep.repTransforms[0].root.gameObject);
            }
        }

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            yield return null;
            if (isNetworked) yield break;

            yield return new WaitForSeconds(3);

            for (int i = 0; Active; i++)
            {
                int reali = i % reps.Count;
                Utilities.MoveAndFacePlayer(reps[reali].repTransforms[0].root.gameObject);
                byte[] posrot = reps[reali].repTransforms[0].root.transform.SerializePosRot();
                SendNetworkData(posrot, new byte[] { (byte)reali });
                yield return new WaitForSeconds(3);
            }
        }
    }

}
