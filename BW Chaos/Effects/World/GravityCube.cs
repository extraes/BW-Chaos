using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Data;
using StressLevelZero.Props.Weapons;

namespace BWChaos.Effects
{
    internal class GravityCube : EffectBase
    {
        public GravityCube() : base("Gravity Cube", 90, EffectTypes.AFFECT_GRAVITY) { }

        private Transform gravObject;
        private Vector3 previousGrav;

        public override void OnEffectStart()
        {
            previousGrav = Physics.gravity;

            Vector3 spawnPosition = Player.rightController.transform.position + Player.rightController.transform.forward;
            gravObject = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            gravObject.position = spawnPosition;
            gravObject.rotation = UnityEngine.Random.rotation;
            Rigidbody rb = gravObject.gameObject.AddComponent<Rigidbody>();
            rb.angularDrag = 0.05f;
            rb.drag = 0.05f;
        }

        public override void OnEffectUpdate() => Physics.gravity = -gravObject.up * 12f;

        public override void OnEffectEnd()
        {
            Physics.gravity = previousGrav;
            GameObject.Destroy(gravObject);
        }
    }
}
