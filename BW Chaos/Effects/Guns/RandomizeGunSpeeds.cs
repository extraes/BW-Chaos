using MelonLoader;
using MelonLoader.Assertions;
using ModThatIsNotMod;
using StressLevelZero.Props.Weapons;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BWChaos.Effects;

internal class RandomizeGunSpeeds : EffectBase
{
    public RandomizeGunSpeeds() : base("Randomize Gun Speeds")
    {
        if (_MH != null)
        {
            MemberInfo del = GetType().GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).First(m => m.Name == nameof(dele));
            Type t = del.DeclaringType;
            int thirtysix = 36;
            Type dt = t.GetNestedType(del.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            id = t.GetMethod(nameof(Init), (BindingFlags)thirtysix).CreateDelegate(dt, this);
            id.DynamicInvoke(new object[] { });

            return;
        }
#if DEBUG && false
                    Chaos.Log("Currently loaded assemblies:");
                    AppDomain.CurrentDomain.GetAssemblies().ForEach(assembly => Chaos.Log(" - " + assembly.GetName().Name));
#endif

        Assembly asm = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "ModThatIsNotMod");
        Type mhc = asm.GetType(new string[] { "ModThatIsNotMod", "Internals", "MelonHashChecking" }.Join("."));
        MethodInfo gmh = mhc.GetMethod("GetMelonHash", BindingFlags.Static | BindingFlags.Public);
        string mh = (string)gmh.Invoke(null, new object[] { MelonUtils.GetApplicationPath() });

#if DEBUG
        Chaos.Log("MTINM.Internals.MHC.GMH => " + mh);
#endif
        _MH = mh;
    }
    static string _MH;
    readonly Delegate id;

    private delegate void dele();
    private void Init()
    {
        // so good char[]
        if (Chaos.isSteamVer) LemonAssert.IsEqual(_MH, "1cf5b055a5dd6be6d15d6db9c0f994fb");
        // so good

        ":)".ToString();
        // this is a comment that compiles. sure it creates some extra garbage but oh well

#if DEBUG
        Chaos.Log("You passed the vibe check");
#endif
    }
    [RangePreference(0, 100, 5)] static readonly float minSpeed = 5f;
    [RangePreference(100, 15_000, 100)] static readonly float maxSpeed = 9000f;


    public override void HandleNetworkMessage(byte[][] data)
    {
        float speed = BitConverter.ToSingle(data[0], 0);
        string path = Encoding.ASCII.GetString(data[1]);

        Gun gun = GameObject.Find(path)?.GetComponent<Gun>();
        if (gun == null)
        {
            Chaos.Warn("Gun not found in client - " + path);
            return;
        }

        gun.SetRpm(speed);
    }

    public override void OnEffectStart()
    {
        if (isNetworked) return;

        foreach (Gun gun in GameObject.FindObjectsOfType<Gun>())
        {
            float ran = Random.Range(minSpeed, maxSpeed);
            SendNetworkData(BitConverter.GetBytes(ran), Encoding.ASCII.GetBytes(gun.transform.GetFullPath()));
            gun.SetRpm(ran);
        }
    }
}
