using Jevil;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VLB;

namespace BLChaos.Effects;

// this was originally going to stop itself after ~90s but unity refused to stop executing the code. why? i dont know
// did i try destroying the component? yes. did i try adding a boolean check? you bet your ass i did. did anything regarding stoppping it work? FUCK no!
internal class RGBLights : EffectBase
{
    // Because instead of making RGBLights open source, I decided to do this. - extraes
    public RGBLights() : base("RGB-ify lights") { }
    [RangePreference(0.25f, 5f, 0.25f)] public static float colorCycleTime = 1f;

    public override void OnEffectStart()
    {
        foreach (Light light in Utilities.FindAll<Light>())
        {
            //if (light.bakingOutput.isBaked) continue; commented because baked lights can have volumetrics
            light.gameObject.AddComponent<UnityLightRGB>();
        }

        // had to overengineer this shit because BONELAB uses postprocess volumes instead of 
        foreach ((Volumetrics vols, GameObject go) in Utilities.FindAll<Volume>().SelectMany(GetComponents))
        {
            UnityVolumetricRGB uvrgb = go.AddComponent<UnityVolumetricRGB>();
            uvrgb.vol = vols;
        }
    }


    private IEnumerable<(Volumetrics, GameObject)> GetComponents(Volume vol)
    {
        // this is stupid lmfao but im just doing this so i can attach the unityvolumetricrgb to the right gameobject
        List<(Volumetrics, GameObject)> volgos = new();

        foreach (VolumeComponent volcom in vol.profile.components)
            if (volcom.GetType() == typeof(Volumetrics))
                volgos.Add(volcom.Cast<Volumetrics>(), vol.gameObject);

        return volgos;
    }


    [RegisterTypeInIl2Cpp]
    public class UnityVolumetricRGB : MonoBehaviour
    {
        public UnityVolumetricRGB(IntPtr ptr) : base(ptr) { }
        public Volumetrics vol;
        ColorParameter color;
        readonly float cycleTime = colorCycleTime;
        public void Start()
        {
            // vol will be set by OnEffectStart
            for (int i = 0; i < vol.parameters.Count; i++)
            {
                if (vol.parameters[i].GetType() == typeof(ColorParameter)) color = vol.parameters[i].Cast<ColorParameter>();
            }
        }

        public void Update()
        {
            if (color.value == Color.white || color.value == Color.black) color.value = Color.cyan;

            Color.RGBToHSV(color.value, out float h, out float s, out float v);

            color.value = Color.HSVToRGB(h + Time.deltaTime * (1 / cycleTime), s, v);
            // todo: check if the volumetric's Albedo color changing even fucking matters when they're baked
        }
    }

    [RegisterTypeInIl2Cpp]
    public class UnityLightRGB : MonoBehaviour
    {
        public UnityLightRGB(IntPtr ptr) : base(ptr) { }

        Light light = null;
        bool foundBeamGeo = false;
        readonly float cycleTime = colorCycleTime;
        public void Start()
        {
            light = gameObject.GetComponent<Light>();
            light.color = Color.cyan;
        }

        public void Update()
        {
            if (light.color == Color.white || light.color == Color.black) light.color = Color.cyan;

            Color.RGBToHSV(light.color, out float h, out float s, out float v);

            light.color = Color.HSVToRGB(h + Time.deltaTime * (1 / cycleTime), s, v);
        }
    }
}