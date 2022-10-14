using System;
using UnityEngine;

namespace BLChaos.Effects;

internal class GetDoxxed : EffectBase
{
    public GetDoxxed() : base("Get Fucking Doxxed ", 15) { }
    private GameObject sign;
    private Transform headT;
    private static AudioClip clip;
    private float dist = 50;
    private float x = 0;
    private float y = 0;
    public override void OnEffectStart()
    {
        clip = clip != null ? clip : GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/vineboom.mp3");

        dist = 50;

        byte[] ipParts = new byte[]
        {
            (byte)UnityEngine.Random.RandomRange(0,256),
            (byte)UnityEngine.Random.RandomRange(0,256),
            (byte)UnityEngine.Random.RandomRange(0,256),
            (byte)UnityEngine.Random.RandomRange(0,256),
        };
        string ip = string.Join(".", ipParts);
        SendNetworkData(ip);
        headT = GlobalVariables.Player_PhysBody.rbHead.transform;
        sign = Utilities.SpawnAd(string.Join(".", ipParts));

        sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * 50;

        sign.GetComponent<Rigidbody>().detectCollisions = false;
        GameObject.Destroy(sign.GetComponent<StressLevelZero.Props.ObjectDestructable>());
        GameObject.Destroy(sign.GetComponent<StressLevelZero.SFX.ImpactSFX>());
        GameObject.Destroy(sign.GetComponent<StressLevelZero.Interaction.InteractableHost>());
    }

    private bool wasFarLastFrame; // BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE 
    public override void OnEffectUpdate()
    {
        if (isNetworked) return;

        // move it closer over a period of 5 seconds
        if (dist > Const.FPI) dist -= Time.deltaTime * 5;
        else dist = Const.FPI; // god forbid if someone lag spikes on the exact frame that this happens

        x = (float)Math.Cos(dist / 3.125f);
        y = (float)Math.Sin(dist / 3.125f);

        sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * dist;
        if (dist > Const.FPI) sign.transform.rotation = Quaternion.Euler(new Vector3(x * 360, y * 360, 0));
        else
        {
            if (wasFarLastFrame) GlobalVariables.SFXPlayer.PlayClip(clip);
            sign.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(headT.forward, Vector3.up));
        }
        wasFarLastFrame = dist > Const.FPI; // BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE 

        // stagger. why? idk. just do it.
        if (Time.frameCount % 4 == 0) SendNetworkData(sign.transform.SerializePosRot());
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        #region Null check and debug log

        if (sign == null)
        {
#if DEBUG
            Chaos.Warn("Sign is null, but it's trying to be moved!");
#endif
            return;
        }

        #endregion

        sign.transform.DeserializePosRot(data);
    }

    public override void HandleNetworkMessage(string data)
    {
        sign = Utilities.SpawnAd(data);
    }
}
