using ModThatIsNotMod;
using System;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace BWChaos;

internal static class Stats
{
#if DEBUG
    private const string modName = "ChaosTesting";
#else
    private const string modName = "Chaos";
#endif
    private const string versionURL = "https://stats.extraes.xyz/increment?mod=" + modName + "&key=" + BuildInfo.Version;
    private const string statBase = "https://stats.extraes.xyz/increment?mod=" + modName + "_Effects&key=";
    //private static string identifier;
    //private static string identifierHashed;
    //private static readonly string data;
    private static readonly HttpClient client = new HttpClient();
    //private static readonly LemonMD5 md5 = new LemonMD5();

    public static void PingVersion()
    {
        if (!Player.handsExist) return;

        if (PlayerPrefs.HasKey("PingedChaos_" + BuildInfo.Version))
        {
            //ready = true;
        }
        else
        {
            try
            {
                client.GetAsync(new Uri(versionURL));
                PlayerPrefs.SetInt("PingedChaos_" + BuildInfo.Version, 1);
                //ready = true;
            }
            catch { }
        }
    }

    public static void EffectCalledManuallyCallback(Effects.EffectBase effect)
    {
        try
        {
            Uri url = new(statBase + effect.GetType().Name);
            client.GetAsync(url);
        }
#if DEBUG
        catch (Exception ex)
        {
            Chaos.Log("Exception was thrown by web client whilst attempting to increment stat for effect " + effect.GetType().Name);
            throw ex;
        }
#else
        catch { }
#endif
    }

    public static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);
        string hexAlphabet = "0123456789ABCDEF";

        foreach (byte b in bytes)
        {
            result.Append(hexAlphabet[b >> 4]);
            result.Append(hexAlphabet[b & 0xF]);
        }

        return result.ToString().ToLower();
    }
}
