using Discord;
using Entanglement.Network;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entanglement.Representation;
using UnityEngine;

namespace BWChaos.Sync
{
    internal class PlayerRepresenting : Effects.EffectBase
    {
        public PlayerRepresenting() : base("The Entities...", 60) { }

        List<PlayerRepresentation> reps = new List<PlayerRepresentation>(16);
        public override void HandleNetworkMessage(byte[][] data)
        {
            int idx = (int)data[1][0];
            var transform = reps[idx].repTransforms[0].root.transform;
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
            foreach (var rep in reps)
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
                var posrot = reps[reali].repTransforms[0].root.transform.SerializePosRot();
                SendNetworkData(posrot, new byte[] { (byte)reali });
                yield return new WaitForSeconds(3);
            }
        }
    }
    
}
