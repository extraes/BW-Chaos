using ModThatIsNotMod;
using System;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class GetDoxxed : EffectBase
    {
        public GetDoxxed() : base("Get Fucking Doxxed ", 15) { }

        private readonly float FPI = (float)Math.PI;
        private GameObject sign;
        private Transform headT;
        private AudioSource soundSource;
        private float dist = 50;
        private float x = 0;
        private float y = 0;
        public override void OnEffectStart()
        {
            if (soundSource == null)
            {
                soundSource = Player.GetPlayerHead().AddComponent<AudioSource>();
                soundSource.bypassEffects = true;
                soundSource.bypassListenerEffects = true;
                soundSource.bypassReverbZones = true;
                soundSource.volume = 0.4f;
            }
            soundSource.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/vineboom.mp3");

            dist = 50;

            byte[] ipParts = new byte[]
            {
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
                (byte)UnityEngine.Random.RandomRangeInt(0,256),
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
            if (dist > FPI) dist -= Time.deltaTime * 5;
            else dist = FPI; // god forbid if someone lag spikes on the exact frame that this happens

            x = (float)Math.Cos(dist / 3.125f);
            y = (float)Math.Sin(dist / 3.125f);

            sign.transform.position = headT.position + Vector3.ProjectOnPlane(headT.forward, Vector3.up).normalized * dist;
            if (dist > FPI) sign.transform.rotation = Quaternion.Euler(new Vector3(x * 360, y * 360, 0));
            else
            {
                if (wasFarLastFrame) soundSource.Play();
                sign.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(headT.forward, Vector3.up));
            }
            wasFarLastFrame = dist > FPI; // BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE BAD CODE 

            // stagger. why? idk. just do it.
            if (Time.frameCount % 4 == 0) SendNetworkData("m" + string.Join(",", sign.transform.position.Serialize(4)) + ";" + string.Join(",", sign.transform.rotation.eulerAngles.Serialize(4)));
        }

        public override void HandleNetworkMessage(string data)
        {
            if (data.StartsWith("m")) MoveSign(data.Substring(1));
            else sign = Utilities.SpawnAd(data);
        }

        private void MoveSign(string data)
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

            string[] vecs = data.Split(';');
            Vector3 pos = Utilities.DeserializeV3(vecs[0]);
            Vector3 eulerRot = Utilities.DeserializeV3(vecs[1]);
            sign.transform.position = pos;
            sign.transform.rotation = Quaternion.Euler(eulerRot);
        }
    }
}
