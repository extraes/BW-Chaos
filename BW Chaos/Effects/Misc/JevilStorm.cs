using System;
using UnityEngine;
using MelonLoader;
using ModThatIsNotMod;
using System.Collections;
using System.Collections.Generic;
using StressLevelZero.Pool;

namespace BWChaos.Effects
{
    internal class JevilStorm : EffectBase
    {
        public JevilStorm() : base("JEVILING", 30) { }
        private const float FTAU = (float)(Math.PI * 2);
        [RangePreference(2, 32, 2)] private static int jevilAmount = 8;
        private static float offset = 0;
        private List<GameObject> jevils = new List<GameObject>();
        

        public override void OnEffectStart() 
        {
            var pool = GameObject.Find("pool - Jevil").GetComponent<Pool>();
            for (int i = 0; i < jevilAmount; i++)
            {
                var poolee = pool.InstantiatePoolee().gameObject;
                //poolee.transform.SetParent(GlobalVariables.Player_PhysBody.rbPelvis.transform);
                poolee.SetActive(true);
                // dont assault ears (too much)
                poolee.GetComponent<AudioSource>().Stop();
                var rb = poolee.GetComponent<Rigidbody>();
                rb.detectCollisions = false;
                rb.isKinematic = true;
                jevils.Add(poolee);
            }
            jevils[0].GetComponent<AudioSource>().Play();
            jevils[jevilAmount / 2].GetComponent<AudioSource>().Play();
        }

        public override void OnEffectUpdate()
        {
            offset += Time.deltaTime;
            Vector3 pos = GlobalVariables.Player_PhysBody.rbPelvis.transform.position;
            for (int i = 0; i < jevilAmount; i++)
            {
                var ofs = FTAU * ((float)i / jevils.Count) + offset;
                var x = Mathf.Cos(ofs);
                var y = Mathf.Sin(ofs);
                var z = Mathf.Abs(Mathf.Sin((ofs + offset) * 2));
                var newPos = pos + new Vector3(x, 0.1f + z, y);
                jevils[i].transform.position = newPos;
                jevils[i].transform.rotation = Quaternion.LookRotation(pos - newPos);
            }
        }

        public override void OnEffectEnd()
        {
            foreach (var jevil in jevils)
            {
                jevil.SetActive(false);
                jevil.GetComponent<Poolee>().isReadyForSpawn = true;
                var rb = jevil.GetComponent<Rigidbody>();
                rb.detectCollisions = true;
                rb.isKinematic = false;
            }
            jevils.Clear();
        }
    }
}
