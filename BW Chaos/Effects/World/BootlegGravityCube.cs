using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Data;
using StressLevelZero.Props.Weapons;

namespace BW_Chaos.Effects
{
    internal class BootlegGravityCube : EffectBase
    {
        public BootlegGravityCube() : base("Bootleg Gravity Cube", 30) { }

        private Transform gravObject;
        private Vector3 previousGrav;

        public override void OnEffectStart()
        {
            previousGrav = Physics.gravity;

            Vector3 spawnPosition = Player.rightController.transform.position + Player.rightController.transform.forward;

            if (CustomItems.customItemsExist)
            {
                SpawnableObject spawnable = null;
                while (spawnable == null
                    || spawnable.title.Contains(".bcm")
                    || spawnable.title.Contains("grenade")
                    || spawnable.prefab.GetComponent<Magazine>())
                    spawnable = CustomItems.GetRandomCustomSpawnable();
                gravObject = GameObject.Instantiate(spawnable.prefab, spawnPosition, UnityEngine.Random.rotation).transform;
            }
            else
            {
                gravObject = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                gravObject.position = spawnPosition;
                gravObject.rotation = UnityEngine.Random.rotation;
            }
        }

        public override void OnEffectUpdate() => Physics.gravity = -gravObject.up * 12f;

        public override void OnEffectEnd()
        {
            Physics.gravity = previousGrav;
            GameObject.Destroy(gravObject);
        }
    }
}
