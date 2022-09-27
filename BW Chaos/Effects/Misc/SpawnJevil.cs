using ModThatIsNotMod;
using ModThatIsNotMod.Nullables;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects;

internal class SpawnJevil : EffectBase
{
    public SpawnJevil() : base("Spawn him.") { }

    AudioSource soundSource;
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
        soundSource.clip = GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/jevil_chaoschaos.wav");

        StressLevelZero.Pool.Pool pool = GameObject.FindObjectsOfType<StressLevelZero.Pool.Pool>().FirstOrDefault(p => p.name == "pool - Jevil");
        GameObject jevil = pool.Spawn(Vector3.zero, Quaternion.identity, null, true);

        SendNetworkData(jevil.transform.SerializePosRot());
        Utilities.MoveAndFacePlayer(jevil);
        //jevil.GetComponent<AudioSource>().outputAudioMixerGroup = GlobalVariables.MusicMixer; // in case changing it on the pool prefab didn't fix it :shrump:
        soundSource.Play();
    }

    public override void HandleNetworkMessage(byte[] data)
    {
        if (!isNetworked) SendNetworkData(data);
        StressLevelZero.Pool.Pool pool = GameObject.FindObjectsOfType<StressLevelZero.Pool.Pool>().FirstOrDefault(p => p.name == "pool - Jevil");
        GameObject jevil = pool.Spawn(Vector3.zero, Quaternion.identity, null, true);
        jevil.transform.DeserializePosRot(data);
    }

}
