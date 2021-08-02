using System;
using UnityEngine;
using MelonLoader;
using VLB;

namespace BWChaos.Effects
{
    internal class RGBLights : EffectBase
    {
        // Because instead of making RGBLights open source, I decided to do this. - extraes
        public RGBLights() : base("Epic Gamer Pride (RGB Lights)", 180) { }

        public override void OnEffectStart() 
        {
            foreach (var light in GameObject.FindObjectsOfType<Light>()) light.gameObject.AddComponent<SuperEpicGamerRGB_Real>();
            // Hopefully this works because it should be ran when the scene is already loaded
            foreach (var volumetric in GameObject.FindObjectsOfType<BeamGeometry>()) volumetric.gameObject.AddComponent<SuperEpicGamerRGB_Working2019_NoVirus>(); 
        }
        
        public override void OnEffectEnd()
        {
            foreach (var rgbLight in GameObject.FindObjectsOfType<SuperEpicGamerRGB_Real>()) GameObject.Destroy(rgbLight);
            foreach (var rgbVolumetric in GameObject.FindObjectsOfType<SuperEpicGamerRGB_Working2019_NoVirus>()) GameObject.Destroy(rgbVolumetric);
        }

        [RegisterTypeInIl2Cpp]
        public class SuperEpicGamerRGB_Real : MonoBehaviour
        {
            public SuperEpicGamerRGB_Real(IntPtr ptr) : base(ptr) { }

            Light light = null;
            float cycleTime = 1f;
            Color originalColor = Color.white;
            void Start()
            {
                light = this.gameObject.GetComponent<Light>();
                originalColor = light.color;
                light.color = Color.cyan;
            }

            void Update()
            {
                if (light.color == Color.white || light.color == Color.black) light.color = Color.cyan;

                Color.RGBToHSV(light.color, out float h, out float s, out float v);

                light.color = Color.HSVToRGB(h + Time.deltaTime * (1 / cycleTime), s, v);
            }

            void Destroy ()
            {
                light.color = originalColor;
            }
        }

        [RegisterTypeInIl2Cpp]
        public class SuperEpicGamerRGB_Working2019_NoVirus : MonoBehaviour
        {
            public SuperEpicGamerRGB_Working2019_NoVirus(IntPtr ptr) : base(ptr) { }

            BeamGeometry beamGeometry = null;
            float alpha = 0f;
            float cycleTime = 1f;
            Color originalColor = Color.white;
            void Start()
            {
                // Save the beamgeometry now so I don't need to call GetComponent every frame & get the alpha now to maintain the intensity of the light
                beamGeometry = this.gameObject.GetComponent<BeamGeometry>();
                alpha = beamGeometry.material.color.a;
                originalColor = beamGeometry.material.color;
                // Set the color to cyan so it can be scrolled in Update(). Without this, any white light would remain white
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

            void Destroy()
            {
                beamGeometry.material.color = originalColor;
            }
        }
    }
}
