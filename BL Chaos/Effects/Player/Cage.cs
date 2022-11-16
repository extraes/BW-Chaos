﻿using UnityEngine;

namespace BLChaos.Effects;

internal class Cage : EffectBase
{
    public Cage() : base("Cage", 15) { }
    static readonly GameObject cagePrefab;
    GameObject cage;
    static Cage()
    {
        cagePrefab = GlobalVariables.EffectResources.LoadAsset("Assets/Cage/cage.prefab").Cast<GameObject>();
        cagePrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        cage.transform.position = Utilities.DebyteV3(data);
    }

    public override void OnEffectStart()
    {
        cage = GameObject.Instantiate(cagePrefab);
        if (isNetworked) return;
        cage.transform.position = GlobalVariables.Player_PhysRig.rbFeet.position;
        SendNetworkData(cage.transform.position.ToBytes());
    }

    public override void OnEffectEnd()
    {
        cage.Destroy();
    }
}
