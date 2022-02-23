using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class Cage : EffectBase
    {
        public Cage() : base("Cage", 15) { }
        static GameObject cagePrefab;
        GameObject cage;
        static Cage()
        {
            cagePrefab = GlobalVariables.EffectResources.LoadAsset<GameObject>("Assets/Cage/cage.prefab");
            cagePrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(cagePrefab);
        }

        public override void HandleNetworkMessage(byte[] data)
        {
            cage.transform.position = Utilities.DebyteV3(data);
        }

        public override void OnEffectStart()
        {
            cage = GameObject.Instantiate(cagePrefab);
            if (isNetworked) return;
            cage.transform.position = GlobalVariables.Player_PhysBody.rbFeet.position;
            SendNetworkData(cage.transform.position.ToBytes());
        }

        public override void OnEffectEnd()
        {
            cage.Destroy();
        }
    }
}
