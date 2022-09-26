using ModThatIsNotMod.Nullables;
using StressLevelZero.Pool;
using System.Collections;
using System.Linq;
using UnityEngine;

// maybe try adding shit from ultrakill?
namespace BWChaos.Effects;

internal class EnemyGauntlet : EffectBase
{
    public EnemyGauntlet() : base("Enemy gauntlet", 120) { }
    [RangePreference(0, 1, 0.05f)] static readonly float volume = 0.3f;

    private static AudioClip clip;
    public override void OnEffectStart()
    {
        clip = clip != null ? clip : GlobalVariables.EffectResources.LoadAsset<AudioClip>("assets/sounds/thecybergrind.mp3");

        GlobalVariables.MusicPlayer.Play(clip, GlobalVariables.MusicMixer, volume, null, null, null);
    }

    public override void OnEffectEnd()
    {
        GlobalVariables.MusicPlayer.Stop();
    }

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        yield return null;
        int entangleChange = Prefs.SyncEffects ? 2 : 1;
        Pool nbPool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Null Body");
        Pool eePool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Ford EarlyExit");
        Pool crabPool = GameObject.FindObjectsOfType<Pool>().FirstOrDefault(p => p.name == "pool - Crablet");


        // spawn nullbodies
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            GameObject spawnedNB = nbPool.Spawn(spawnPos, spawnRot, null, true);
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(10f);

        // spawn a bunch of crablets
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            GameObject spawnedCrab = crabPool.Spawn(spawnPos, spawnRot, null, true);
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(10f);

        // spawn a bunch of earlyexits
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysBody.feet.transform.position;
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            GameObject spawnedEE = eePool.Spawn(spawnPos, spawnRot, null, true); //todo: change to earlyexit that can throw stuff
            //spawnedEE.GetComponent<AIBrain>().behaviour.enableThrowAttack = true;
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(15f);
        GlobalVariables.MusicPlayer.Stop();
        ForceEnd();
    }
}
