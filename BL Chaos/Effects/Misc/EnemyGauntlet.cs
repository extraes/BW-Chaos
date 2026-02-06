#if !NOBONELIB
// maybe try adding shit from ultrakill?
namespace BLChaos.Effects;

internal class EnemyGauntlet : EffectBase
{
    public EnemyGauntlet() : base("Enemy gauntlet", 120) { }
    [RangePreference(0, 1, 0.05f)] static float volume = 0.3f;

    private static AudioClip clip;
    public override void OnEffectStart()
    {
        clip = clip != null ? clip : GlobalVariables.EffectResources.LoadAsset("assets/sounds/thecybergrind.mp3").Cast<AudioClip>();

        var nulledBool = new Il2CppSystem.Nullable<bool>(false);
        nulledBool.hasValue = false;
        var nulledFloat = new Il2CppSystem.Nullable<float>(0);
        nulledFloat.hasValue = false;

        var vol = new Il2CppSystem.Nullable<float>(volume);
        vol.hasValue = true;
        GlobalVariables.MusicPlayer.Play(clip, GlobalVariables.MusicMixer, vol, nulledBool, nulledFloat, nulledFloat);
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
        int entangleChange = Prefs.syncEffects ? 2 : 1;
        Spawnable nullbodies = Barcodes.ToSpawnable(JevilBarcode.NULL_BODY);
        Spawnable earlyExit = Barcodes.ToSpawnable(JevilBarcode.EARLY_EXIT_ZOMBIE);
        Spawnable crablets = Barcodes.ToSpawnable(JevilBarcode.CRABLET);


        // spawn nullbodies
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysRig.feet.transform.position;
            float theta = (i / 8f) * Const.FPI * 2; // no way its ftau
            float x = Mathf.Cos(theta);
            float y = Mathf.Sin(theta);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            nullbodies.Spawn(spawnPos, spawnRot, true);
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(10f);

        // spawn a bunch of crablets
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysRig.feet.transform.position;
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            crablets.Spawn(spawnPos, spawnRot, true);
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(10f);

        // spawn a bunch of earlyexits
        for (int i = 0; i < 4; i++)
        {
            if (i % entangleChange != 0) continue;
            Vector3 playerPos = GlobalVariables.Player_PhysRig.feet.transform.position;
            float theta = (i / 8f) * 360;
            float x = Mathf.Cos(theta * Const.FPI / 180);
            float y = Mathf.Sin(theta * Const.FPI / 180);

            Vector3 spawnPos = playerPos + new Vector3(x, 0.1f, y);
            Quaternion spawnRot = Quaternion.LookRotation(spawnPos - playerPos, new Vector3(0, 1, 0));
            earlyExit.Spawn(spawnPos, spawnRot, true); //todo: change to earlyexit that can throw stuff
            //spawnedEE.GetComponent<AIBrain>().behaviour.enableThrowAttack = true;
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(15f);
        GlobalVariables.MusicPlayer.Stop();
        ForceEnd();
    }
}
#endif
