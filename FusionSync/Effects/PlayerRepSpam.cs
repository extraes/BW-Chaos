using BLChaos.Effects;
using Il2CppSLZ.Marrow;
using Jevil;
using LabFusion.Representation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BLChaos.Sync.Effects;

public class PlayerRepSpam : EffectBase
{
    public PlayerRepSpam() : base("Friends!", 30, EffectTypes.LAGGY) { }

    readonly List<RigManager> reps = new(8);

    public override void HandleNetworkMessage(byte[][] data)
    {
        int idx = data[1][0];
        Transform transform = reps[idx].transform.root;
        transform.DeserializePosRot(data[0]);
    }

    public override void OnEffectEnd()
    {
        foreach (RigManager rep in reps)
        {
            GameObject.Destroy(rep.transform.root.gameObject);
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
            RigManager rig = reps[reali];
            if (rig.INOC())
            {
                Log("Creating new rig @ idx " + reali);
                PlayerRepUtilities.CreateNewRig(rm => reps[reali] = rm);

                while (reps[reali].INOC())
                    yield return null;

                rig = reps[reali];
            }

            Utilities.MoveAndFacePlayer(reps[reali].transform.root.gameObject);
            byte[] posrot = rig.transform.root.SerializePosRot();
            SendNetworkData(posrot, new byte[] { (byte)reali });
            yield return new WaitForSeconds(3);
        }
    }
}
