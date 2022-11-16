﻿using BoneLib;
using System;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace BLChaos;

internal static class Stats
{
#if DEBUG
    private const string modName = "BLChaosTesting";
#else
    private const string modName = "BLChaos";
#endif
    private const string versionURL = "https://stats.extraes.xyz/increment?mod=" + modName + "&key=" + BuildInfo.Version;
    private const string statBase = "https://stats.extraes.xyz/increment?mod=" + modName + "_Effects&key=";
    private const string prefsKey = "Pinged" + modName + "V" + BuildInfo.Version;
    private static readonly HttpClient client = new();

    public static void PingVersion()
    {
        if (!Player.handsExist) return;

        if (!PlayerPrefs.HasKey(prefsKey))
        {
            try
            {
                client.GetAsync(new Uri(versionURL));
                PlayerPrefs.SetInt(prefsKey, 1);
            }
#if !DEBUG
            catch { }
#else
            catch (Exception ex)
            {
                Chaos.Warn("Failed to ping stats server OR Failed to set player pref!");
                Chaos.Warn(ex);
            }
#endif
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
}