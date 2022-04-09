using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace BWChaos.Effects
{
    internal class GarbageTextures : EffectBase
    {
        static Texture2D[] textures;
        [RangePreference(0f, 1f, 0.02f)] static float swapChance = 0.2f;
        public GarbageTextures() : base("Garble Random Textures") { Init(); }

        private void Init()
        {
#if DEBUG
            uint totalSize = 0;
            uint totalSizeConverted = 0;
#endif
            // why 256? dunno, but its a large enough number that *maybe* people wont notice repeats!
            const int texCount = 256;

            textures = new Texture2D[256];

            // Generate a bunch of random bytes, 128 * 128 * 3 (Length, width, RGB)
            int texWidth = 128;
            int texHeight = 128;
            int texDepth = 3;
            System.Random rand = new System.Random();

            for (int i = 0; i < texCount; i++)
            {
                // Calculate the amount of data needed to fill a 128x128 texture
                byte[] data = new byte[texWidth * texHeight * texDepth];

                #region Convert raw bytes to jpg

                // Unity doesn't load raw BMP's, so we need to convert it to a JPG/PNG/Supported format. How? IDK, but the internet has a way!
                // https://stackoverflow.com/questions/23781364/generating-a-random-jpg-image-from-console-application/23812460#23812460
                using (Bitmap bitmap = new Bitmap(texWidth, texHeight, PixelFormat.Format24bppRgb))
                {
                    // 2. Get access to the raw bitmap data
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                    // 3. Generate RGB noise and write it to the bitmap's buffer.
                    // Note that we are assuming that data.Stride == 3 * data.Width for simplicity/brevity here.
                    byte[] noise = new byte[bmpData.Width * bmpData.Height * 3];
                    rand.NextBytes(noise);
                    Marshal.Copy(noise, 0, bmpData.Scan0, noise.Length);

#if DEBUG
                    totalSize += (uint)noise.Length;
#endif

                    bitmap.UnlockBits(bmpData);

                    // 4. Save as JPEG and copy to array
                    using (MemoryStream jpegStream = new MemoryStream())
                    {
                        bitmap.Save(jpegStream, ImageFormat.Jpeg);
                        data = jpegStream.ToArray();
                    }
                }

                #endregion

                Texture2D tex = new Texture2D(256, 256);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, data);
                tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
                textures[i] = tex;

#if DEBUG
                totalSizeConverted += (uint)data.Length;
#endif
            }

#if DEBUG
            Chaos.Log("Generated " + totalSize + " bytes of noise data, converted to " + totalSizeConverted + " bytes of valid jpeg");
            Chaos.Log($"{totalSize / 1024}KB, {totalSizeConverted / 1024}KB");
#endif
        }

        public override void OnEffectStart()
        {
            if (textures == null || textures[0] == null) Init();

            if (isNetworked) return;

            foreach (var mesh in GameObject.FindObjectsOfType<MeshRenderer>())
            {
                if (Random.value < swapChance)
                {
                    if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                    if (mesh.GetComponent<TMPro.TMP_Text>() != null) continue;
                    mesh.material.SetTexture("_MainTex", textures.Random());
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

            var mesh = GameObject.Find(path)?.GetComponent<MeshRenderer>();
            if (mesh == null)
            {
                Chaos.Warn("GameObject/MeshRenderer was not found in client: " + path);
            }
            else
            {
                mesh.material.SetTexture("_MainTex", textures.Random());
                mesh.material.color = col;
            }
        }
    }
}
