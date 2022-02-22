using Boneworks;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BWChaos.Effects
{
    internal class ScrollingTextures : EffectBase
    {
        public ScrollingTextures() : base("IN YOUR WALLS.", 5) { }
        [RangePreference(0, 1,0.0125f)] static float scrollSpeedX = 0.025f;
        [RangePreference(0, 1, 0.0125f)] static float scrollSpeedY = 0.05f;
        [RangePreference(0, 1, 0.02f)] static float swapChance = 0.2f;

        [AutoCoroutine]
        public IEnumerator CoRun()
        {
            if (isNetworked) yield break;
            bool stagger = true;

            foreach (var mesh in Utilities.FindAll<MeshRenderer>())
            {
                if (mesh.gameObject.GetComponent<Treadmill>() != null) continue;
                if (mesh == null || !mesh.gameObject.active) continue;
                
                if (Random.value < swapChance)
                {
                    if (mesh.name.ToLower().Contains("text") || mesh.name.ToLower().Contains("ui")) continue;
                    if (mesh.GetComponent<TMPro.TMP_Text>() != null) continue;


                    var tread = mesh.gameObject.AddComponent<Treadmill>();
                    tread.directionMill = new Vector2(scrollSpeedX, scrollSpeedY);
                    tread.materialMill = mesh?.material;
                    SendNetworkData(mesh.transform.GetFullPath());
                    if (stagger = !stagger) yield return null;
                }
            }
        }

        public override void HandleNetworkMessage(string data)
        {
            string[] args = data.Split(';');
            

            var go = GameObject.Find(args[1]);
            if (go == null)
            {
                Chaos.Warn("GameObject was not found in client: " + args[1]);
                return;
            }

            var mesh = go.GetComponent<MeshRenderer>();
            if (mesh == null)
            {
                Chaos.Warn("The recieved GameObject didn't have a MeshRenderer");
                return;
            }
            var tread = mesh.gameObject.AddComponent<Treadmill>();
            tread.materialMill = mesh.material;
            tread.directionMill = new Vector2(0.025f, 0.05f);
        }
    }
}
