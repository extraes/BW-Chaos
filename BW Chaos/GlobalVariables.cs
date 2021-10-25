﻿using System.Collections.Generic;
using UnityEngine;
using WatsonWebsocket;
using BWChaos.Effects;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using UnityEngine.Audio;

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
        
        public static Camera[] Cameras; // because oculusvr loves COCK i cant have a single camera reference, zuck please get the schlong out ya mouf if you please (nc = 0.001; fc = 10000;)

        public static AudioMixerGroup MusicMixer;

        public static AssetBundle EffectResources;
    }
}
