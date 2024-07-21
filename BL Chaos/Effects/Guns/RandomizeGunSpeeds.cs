using BoneLib;
using Cysharp.Threading.Tasks;
using Jevil;
using Jevil.Spawning;
using MelonLoader;
using MelonLoader.Assertions;
using SLZ.Marrow.Pool;
using SLZ.Props.Weapons;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class RandomizeGunSpeeds : EffectBase
{
    public RandomizeGunSpeeds() : base("Randomize Gun Speeds") { }
    [RangePreference(0, 100, 5)] static readonly float minSpeed = 5f;
    [RangePreference(250, 15_000, 250)] static readonly float maxSpeed = 9000f;


    public override void HandleNetworkMessage(byte[][] data)
    {
        float speed = BitConverter.ToSingle(data[0], 0);
        string path = Encoding.ASCII.GetString(data[1]);

        Gun gun = GameObject.Find(path)?.GetComponent<Gun>();
        if (gun == null)
        {
            Chaos.Warn("Gun not found in client - " + path);
            return;
        }

        gun.SetRpm(speed);
    }

    public override void OnEffectStart()
    {
        if (isNetworked) return;

        foreach (Gun gun in GameObject.FindObjectsOfType<Gun>())
        {
            float ran = Random.Range(minSpeed, maxSpeed);
            SendNetworkData(BitConverter.GetBytes(ran), Encoding.ASCII.GetBytes(gun.transform.GetFullPath()));
            gun.SetRpm(ran);
        }
    }

#if DEBUG
    internal override async Task<TestResult> Test()
    {
        AssetPoolee poolee = await Barcodes.ToSpawnable(JevilBarcode.MP5).SpawnAsync(Vector3.one, Quaternion.identity);
        Gun newGun = poolee.GetComponentInChildren<Gun>();
        float preRPM = newGun.roundsPerMinute;

        newGun.ForceFireable();
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        OnEffectStart();

        float postRPM = newGun.roundsPerMinute;

        Log($"PreRPM={preRPM}, PostRPM={postRPM}");
        return Res(postRPM != preRPM);
    }
#endif
}
