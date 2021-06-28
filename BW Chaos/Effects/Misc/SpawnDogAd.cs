using System;
using TMPro;
using HarmonyLib;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.RandomShit;

namespace BW_Chaos.Effects
{
    internal class SpawnDogAd : EffectBase
    {
        public SpawnDogAd() : base("Spawn Dog Ad") { }

        public override void OnEffectStart()
        {
			// todo: i have zero fucking clue if this works
			byte[] dogBytes = null;
			AdManager.GetDogBytes((b) =>{ dogBytes = b; });
			// taken from mtinm, CreateDogAd doesnt make the actual ad it seems and this stuff is in a coroutine
			Texture2D texture = new Texture2D(2, 2);
			ImageConversion.LoadImage(texture, dogBytes);
			// this line uses some reflection magic to get a `private` variable
			GameObject newAd = GameObject.Instantiate((GameObject)typeof(AdManager).GetField("baseAd", AccessTools.all).GetValue(null));
			newAd.GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);
			MeshRenderer renderer = newAd.GetComponentInChildren<MeshRenderer>();
			Material mat = renderer.material;
			mat.mainTexture = texture;
			mat.color = Color.white;
			Vector3 curScale = renderer.transform.localScale;
			Vector3 newScale = new Vector3(texture.width / texture.height * curScale.y, curScale.y, curScale.z);
			renderer.transform.localScale = newScale;
			renderer.transform.Rotate(renderer.transform.up, 180f);
			foreach (BoxCollider col in newAd.GetComponentsInChildren<BoxCollider>())
				col.size = newScale;
			newAd.SetActive(true);
			newAd.transform.position = Player.GetPlayerHead().transform.position + Player.GetPlayerHead().transform.forward * 2f;
			newAd.transform.rotation = Quaternion.LookRotation(newAd.transform.position - Player.GetPlayerHead().transform.position);
		}
    }
}
