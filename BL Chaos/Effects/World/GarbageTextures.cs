using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine.Experimental.Rendering;
using Color = UnityEngine.Color;

namespace BLChaos.Effects;

internal class GarbageTextures : EffectBase
{
    // why 256? dunno, but its a large enough number that *maybe* people wont notice repeats!
    static readonly int texCount = Chaos.isQuest ? 64 : 256;
    static readonly int texWidth = Chaos.isQuest ? 64 : 128;
    static readonly int texHeight = Chaos.isQuest ? 64 : 128;
    static readonly int texDepth = 3;
    static Texture2D[] textures = new Texture2D[texCount];

    [RangePreference(0f, 1f, 0.02f)] static float swapChance = 0.2f;
    public GarbageTextures() : base("Garble Random Textures") { Init(); }

    private void Init()
    {
#if DEBUG
        uint totalSize = 0;
        uint totalSizeConverted = 0;
#endif
        
        System.Random rand = new System.Random();

        for (int i = 0; i < texCount; i++)
        {
            // Calculate the amount of data needed to fill a 128x128 texture
            byte[] data = new byte[texWidth * texHeight * texDepth];
            rand.NextBytes(data);

            // Unity doesn't load raw BMP's, so we need to convert it to a JPG/PNG/Supported format. How? IDK, but the internet has a way!
            //byte[] image = ConvertBMPToJPG(data);
            var unhollowerArray = (Il2CppStructArray<byte>)data;
            var il2cppSystemArray = new Il2CppSystem.Array(unhollowerArray.Pointer);
            byte[] image = ImageConversion.EncodeArrayToJPG(il2cppSystemArray, GraphicsFormat.R8G8B8_SRGB, (uint)texWidth, (uint)texHeight);
#if DEBUG
            totalSize += (uint)image.Length;
#endif
            Texture2D tex = new Texture2D(2, 2);
            tex.filterMode = FilterMode.Point;
            ImageConversion.LoadImage(tex, image);
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
            textures[i] = tex;

#if DEBUG
            totalSizeConverted += (uint)image.Length;
#endif
        }

#if DEBUG
        Log("Generated " + totalSize + " bytes of noise data, converted to " + totalSizeConverted + " bytes of valid jpeg");
        Log($"{totalSize / 1024}KB, {totalSizeConverted / 1024}KB");
#endif
    }

    public override void OnEffectStart()
    {
        if (textures == null || textures[0] == null) Init();

        if (isNetworked) return;

        foreach (MeshRenderer mesh in GameObject.FindObjectsOfType<MeshRenderer>())
        {
            if (Random.value < swapChance)
            {
                if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                if (mesh.GetComponent<TMP_Text>() != null) continue;
                mesh.material.SetTexture("_MainTex", textures!.Random());
                Color col = Random.ColorHSV();
                mesh.material.color = col;

                byte[][] data = new byte[][]
                {
                    BitConverter.GetBytes(col.r),
                    BitConverter.GetBytes(col.g),
                    BitConverter.GetBytes(col.b),
                    BitConverter.GetBytes(col.a),
                    Encoding.ASCII.GetBytes(mesh.transform.GetFullPath())
                };

                SendNetworkData(data.Flatten()); // lets be real, the color is the part that makes the most difference
            }
        }
    }

    public override void HandleNetworkMessage(byte[][] data)
    {
        float[] colors = new float[] {
            BitConverter.ToSingle(data[0], 0),
            BitConverter.ToSingle(data[0], sizeof(float) * 1),
            BitConverter.ToSingle(data[0], sizeof(float) * 2),
            BitConverter.ToSingle(data[0], sizeof(float) * 3),
        };
        string path = Encoding.ASCII.GetString(data[1]);
        Color col = new Color(colors[0], colors[1], colors[2], colors[3]);

        MeshRenderer mesh = GameObject.Find(path)?.GetComponent<MeshRenderer>();
        if (mesh == null)
        {
            Chaos.Warn("GameObject/MeshRenderer was not found in client: " + path);
        }
        else
        {
            mesh.material.SetTexture(Const.URP_MAINTEX_NAME, textures.Random());
            mesh.material.color = col;
        }
    }
}
