using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace BLChaos.Effects;

internal class VideoTextures : EffectBase
{
    static GameObject playerParent;
    static VideoClip[] videos;
    static VideoPlayer[] videoPlayers;
    static RenderTexture[] textures;
    static Material[] materials;
    static readonly Shader vrstandard = Shader.Find("Valve/vr_standard");
    [RangePreference(0f, 1f, 0.01f)] static readonly float swapChance = 0.2f;
    // changing 2300 textures at once crashed my game (in sewers) lmao
    [RangePreference(25, 2000, 25)] static readonly int limit = 500; // lest the game crash once more
    public VideoTextures() : base("Video textures", EffectTypes.LAGGY) { }


    private void Init()
    {
        playerParent = new GameObject("BWChaos Video Parent");
        playerParent.hideFlags = HideFlags.DontUnloadUnusedAsset;
        GameObject.DontDestroyOnLoad(playerParent);

        videos = GlobalVariables.ResourcePaths
            .Where(a => a.Contains("/videoswap/"))
            .Select(a => GlobalVariables.EffectResources.LoadAsset<VideoClip>(a))
            .ToArray();

        videoPlayers = new VideoPlayer[videos.Length];
        textures = new RenderTexture[videos.Length];
        materials = new Material[videos.Length];
        for (int i = 0; i < videoPlayers.Length; i++)
        {
#if DEBUG
            Log("Generating videoplayer and rendertexture for " + videos[i].name);
#endif
            videoPlayers[i] = playerParent.AddComponent<VideoPlayer>();
            videoPlayers[i].clip = videos[i];
            videoPlayers[i].isLooping = true;
            videoPlayers[i].SetDirectAudioMute(0, true);
            textures[i] = new RenderTexture(256, 256, 24);
            //textures[i].hideFlags = HideFlags.DontUnloadUnusedAsset;
            textures[i].wrapMode = TextureWrapMode.Repeat;
            textures[i].name = videos[i].name;
            GameObject.DontDestroyOnLoad(textures[i]);
            videoPlayers[i].targetTexture = textures[i];

            materials[i] = new Material(vrstandard);
            materials[i].name = textures[i].name;
            //materials[i].hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(materials[i]);
            materials[i].mainTexture = textures[i];
        }

#if DEBUG
        Log($"Created {textures.Length} render textures for {Name}");
#endif
    }

    public override void OnEffectStart()
    {
        if (playerParent == null || textures == null || textures[0] == null) Init();

        playerParent.SetActive(true);

        if (isNetworked) return;

        UnhollowerBaseLib.Il2CppArrayBase<MeshRenderer> rends = GameObject.FindObjectsOfType<MeshRenderer>();
        int count = 0;
        foreach (MeshRenderer mesh in rends)
        {
            if (Random.value < swapChance)
            {
                if (count++ > limit) return; // inline incrementing
                if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                if (mesh.GetComponent<TMPro.TMP_Text>() != null) continue;

                Material mat = materials.Random();
                SendNetworkData(mat.name + ";" + mesh.transform.GetFullPath());

                mesh.material = mat;
            }
        }
    }

    public override void HandleNetworkMessage(string data)
    {
        string[] args = data.Split(';');
        Material mat = materials.FirstOrDefault(m => m.name == args[0]);

        GameObject go = GameObject.Find(args[1]);
        // null checks pog
        if (go == null)
        {
            Chaos.Warn("GameObject was not found in client: " + args[1]);
            return;
        }
        if (mat == null)
        {
            Chaos.Error("Material was not found in client: " + args[0]);
            Chaos.Error("(Are you a lite user trying to play with regular users?)");
            return;
        }


        MeshRenderer mesh = go.GetComponent<MeshRenderer>();
        mesh.material = mat;
    }
}
