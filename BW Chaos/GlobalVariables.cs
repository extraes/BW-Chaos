﻿using System.Collections.Generic;
using UnityEngine;
using WatsonWebsocket;
using BW_Chaos.Effects;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;

namespace BW_Chaos
{
    internal static class GlobalVariables
    {
        public static WatsonWsClient WatsonClient;
        public static List<EffectBase> ActiveEffects = new List<EffectBase>();
        public static List<EffectBase> CandidateEffects = new List<EffectBase>();

        public static GameObject WristChaosUI;
        public static GameObject OverlayChaosUI;

        public static BodyVitals Player_BodyVitals;
        public static RigManager Player_RigManager;
        public static Player_Health Player_Health;
        public static PhysBody Player_PhysBody;
    }
}
