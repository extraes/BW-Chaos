﻿using BWChaos.Effects;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using WatsonWebsocket;

namespace BWChaos
{
    internal static class GlobalVariables
    {
        // The WatsonClient MAY BE NULL, so dont get caught lacking!
        public static WatsonWsClient WatsonClient;
        public static List<EffectBase> ActiveEffects = new List<EffectBase>();
        public static List<EffectBase> CandidateEffects = new List<EffectBase>();
        public static List<EffectBase> PreviousEffects = new List<EffectBase>();

        public static GameObject WristChaosUI;
        public static GameObject OverlayChaosUI;

        public static BodyVitals Player_BodyVitals;
        public static RigManager Player_RigManager;
        public static Player_Health Player_Health;
        public static PhysBody Player_PhysBody;

        public static Camera SpectatorCam;
        public static Camera[] Cameras;

        public static AudioMixerGroup MusicMixer;

        public static AssetBundle EffectResources;
        public static IReadOnlyList<string> ResourcePaths;

        public static Chaos thisChaos; // so that we can access some instanced fields, like harmonylib for easy patching & unpatching
    }
}
