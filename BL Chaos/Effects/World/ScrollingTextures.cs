using Il2CppSLZ.Bonelab;

namespace BLChaos.Effects;

internal class ScrollingTextures : EffectBase
{
    public ScrollingTextures() : base("IN YOUR WALLS.", 5) { }
    [RangePreference(0, 1, 0.0125f)] static float scrollSpeedX = 0.025f;
    [RangePreference(0, 1, 0.0125f)] static float scrollSpeedY = 0.05f;
    [RangePreference(0, 1, 0.0125f)] static float scrollSpeedVariance = 0.025f;
    [RangePreference(0, 1, 0.05f)] static float swapChance = 0.2f;

    [AutoCoroutine]
    public IEnumerator CoRun()
    {
        if (isNetworked) yield break;
        bool stagger = true;

        foreach (MeshRenderer mesh in Utilities.FindAll<MeshRenderer>())
        {
            if (mesh.gameObject.GetComponent<Treadmill>() != null) continue;
            if (mesh == null || !mesh.gameObject.active) continue;

            if (Random.value < swapChance)
            {
                if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                if (mesh.GetComponent<TMP_Text>() != null) continue;


                Treadmill tread = mesh.gameObject.AddComponent<Treadmill>();
                Vector2 scrollSpeed = new Vector2(scrollSpeedX, scrollSpeedY) + (new Vector2(scrollSpeedVariance, scrollSpeedVariance) * Random.insideUnitCircle);
                tread.directionMill = scrollSpeed;
                tread.materialMill = mesh.sharedMaterial;
                SendNetworkData(mesh.transform.GetFullPath());
                if (stagger = !stagger) yield return null;
            }
        }
    }

    public override void HandleNetworkMessage(string data)
    {
        GameObject go = GameObject.Find(data);
        if (go == null)
        {
            Chaos.Warn("GameObject was not found in client: " + data);
            return;
        }

        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        if (mesh == null)
        {
            Chaos.Warn("The recieved GameObject didn't have a MeshRenderer");
            return;
        }
        Treadmill tread = mesh.gameObject.AddComponent<Treadmill>();
        tread.materialMill = mesh.material;
        Vector2 scrollSpeed = new Vector2(scrollSpeedX, scrollSpeedY) + (new Vector2(scrollSpeedVariance, scrollSpeedVariance) * Random.insideUnitCircle);
        tread.directionMill = scrollSpeed;
    }
}
