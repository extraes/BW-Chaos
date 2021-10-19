using MelonLoader;
using System;
using UnityEngine;
using VLB;

namespace BWChaos.Effects
{
    // this was originally going to stop itself after ~90s but unity refused to stop executing the code. why? i dont know
    // did i try destroying the component? yes. did i try adding a boolean check? you bet your ass i did. did anything regarding stoppping it work? FUCK no!
    internal class RGBLights : EffectBase
    {
        // Because instead of making RGBLights open source, I decided to do this. - extraes
        public RGBLights() : base("RGB-ify lights") { }

        public override void OnEffectStart()
        {
            foreach (var light in GameObject.FindObjectsOfType<Light>())
            {
                //if (light.bakingOutput.isBaked) continue; commented because baked lights can have volumetrics
                light.gameObject.AddComponent<UnityLightRGB>();
            }
            // Hopefully this works because it should be ran when the scene is already loaded
            //foreach (var volumetric in GameObject.FindObjectsOfType<BeamGeometry>()) volumetric.gameObject.AddComponent<VolumetricRGB>(); 
        }

    }
    [RegisterTypeInIl2Cpp]
    public class UnityLightRGB : MonoBehaviour
    {
        public UnityLightRGB(IntPtr ptr) : base(ptr) { }

        Light light = null;
        bool foundBeamGeo = false;
        float cycleTime = 1f;
        void Start()
        {
            light = this.gameObject.GetComponent<Light>();
            light.color = Color.cyan;
        }

        void Update()
        {
            if (light.color == Color.white || light.color == Color.black) light.color = Color.cyan;

            if (foundBeamGeo == false)
            {
                if (this.gameObject.GetComponent<VolumetricLightBeam>() == null) foundBeamGeo = true;
                else
                {
                    var beamGeo = this.gameObject.GetComponentInChildren<BeamGeometry>();
                    if (beamGeo != null)
                    {
                        beamGeo.gameObject.AddComponent<VolumetricRGB>();
                        foundBeamGeo = true;
                    }
                }
            }
            Color.RGBToHSV(light.color, out float h, out float s, out float v);

            light.color = Color.HSVToRGB(h + Time.deltaTime * (1 / cycleTime), s, v);
        }
    }

    [RegisterTypeInIl2Cpp]
    public class VolumetricRGB : MonoBehaviour
    {
        public VolumetricRGB(IntPtr ptr) : base(ptr) { }

        BeamGeometry beamGeometry = null;
        float alpha = 0f;
        float cycleTime = 1f;
        void Start()
        {
            beamGeometry = this.gameObject.GetComponent<BeamGeometry>();
            alpha = beamGeometry.material.color.a;
            beamGeometry.material.color = Color.cyan;
        }

        void Update()
        {
            var c = beamGeometry.material.color;
            c.a = 1;
            if (c == Color.white || c == Color.black) beamGeometry.material.color = Color.cyan;
            Color.RGBToHSV(beamGeometry.material.color, out float h, out float s, out float v);

            var color = Color.HSVToRGB(h + Time.deltaTime * (1 / cycleTime), s, v);
            color.a = alpha;
            beamGeometry.material.color = color;
        }
    }
}
