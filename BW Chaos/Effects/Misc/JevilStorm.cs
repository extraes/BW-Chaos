using StressLevelZero.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BWChaos.Effects;

internal class JevilStorm : EffectBase
{
    public JevilStorm() : base("JEVILING", 30) { }
    private const float FTAU = (float)(Math.PI * 2);
    [RangePreference(2, 32, 2)] private static readonly int jevilAmount = 8;
    private static float offset = 0;
    private readonly List<GameObject> jevils = new List<GameObject>();


    public override void OnEffectStart()
    {
        Pool pool = GameObject.Find("pool - Jevil").GetComponent<Pool>();
        for (int i = 0; i < jevilAmount; i++)
        {
            GameObject poolee = pool.InstantiatePoolee().gameObject;
            //poolee.transform.SetParent(GlobalVariables.Player_PhysBody.rbPelvis.transform);
            poolee.SetActive(true);
            // dont assault ears (too much)
            poolee.GetComponent<AudioSource>().Stop();
            Rigidbody rb = poolee.GetComponent<Rigidbody>();
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
            float ofs = FTAU * ((float)i / jevils.Count) + offset;
            float x = Mathf.Cos(ofs);
            float y = Mathf.Sin(ofs);
            float z = Mathf.Abs(Mathf.Sin((ofs + offset) * 2));
            Vector3 newPos = pos + new Vector3(x, 0.1f + z, y);
            jevils[i].transform.position = newPos;
            jevils[i].transform.rotation = Quaternion.LookRotation(pos - newPos);
        }
    }

    public override void OnEffectEnd()
    {
        foreach (GameObject jevil in jevils)
        {
            jevil.SetActive(false);
            jevil.GetComponent<Poolee>().isReadyForSpawn = true;
            Rigidbody rb = jevil.GetComponent<Rigidbody>();
            rb.detectCollisions = true;
            rb.isKinematic = false;
        }
        jevils.Clear();
    }
}
