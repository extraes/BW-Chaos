using Il2CppSLZ.Marrow.Audio;

namespace BLChaos;

public static class Extensions
{
    static readonly string[] Newline = { Environment.NewLine, };
    public static string RemoveFirstLines(this string s, int n)
    {
        string[] lines = s
            .Split(Newline, StringSplitOptions.None)
            .Skip(n)
            .ToArray();

        string output = string.Join(System.Environment.NewLine, lines);
        return output;
    }

    public static void Reset(this Transform transform)
    {
        //transform.position = Vector3.zero;
        //transform.rotation = Quaternion.identity;

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    public static bool InHierarchyOf(this Transform t, string parentName)
    {
        if (t.name == parentName)
            return true;

        if (t.parent == null)
            return false;

        t = t.parent;

        return InHierarchyOf(t, parentName);
    }

    public static bool IsChildOfRigManager(this Transform t) => InHierarchyOf(t, GlobalVariables.Player_RigManager.name);

    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> fun)
    {
        foreach (T item in sequence) fun(item);
    }

    public static float Interpolate(this (float, float) tuple, float time)
    {

        float clampTime = Mathf.Clamp(time, 0, 1);
        float correctedCos = -(Mathf.Cos(clampTime * (float)Math.PI) - 1) / 2;
        float diff = tuple.Item2 - tuple.Item1;
        return tuple.Item1 + (diff * correctedCos);
    }

    public static void Destroy(this GameObject go) => GameObject.Destroy(go);

    public static void UseEmbeddedResource(this System.Reflection.Assembly assembly, string resourcePath, Action<byte[]> whatToDoWithResource)
    {
        using Stream stream = assembly.GetManifestResourceStream(resourcePath) ?? throw new Exception("Path not found: " + resourcePath);
        using MemoryStream mStream = new((int)stream.Length); // Don't overallocate memory to the mstream (?)
        // Copy the stream to a memorystream. Why? Don't know, ask .NET designers.
        stream.CopyTo(mStream);
        whatToDoWithResource(mStream.ToArray());
    }

    //public static T Random<T>(this IEnumerable<T> sequence)
    //{
    //    return sequence.ElementAt(UnityEngine.Random.Range(0, sequence.Count()));
    //}

    //// Random w/ IEnumerable doesn't check for arrays, only ICollection
    //public static T Random<T>(this T[] sequence)
    //{
    //    return sequence[UnityEngine.Random.Range(0, sequence.Length)];
    //}

    public static string GetFullPath(this Transform t)
    {
        string path = "/" + t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = "/" + t.name + path;
        }
        return path;
    }

    [Obsolete("Use " + nameof(ToBytes) + " and send Vector3s as bytes instead")]
    public static string[] Stringify(this Vector3 vec, int decimals, char delimiter = ',')
    {
        float[] floats = new float[] { vec.x, vec.y, vec.z, };
        string[] strings = floats.Select(f => Math.Round(f, decimals).ToString()).ToArray();
        return strings;
    }

    public static byte[] ToBytes(this Vector3 vec)
    {
        byte[][] bytess = new byte[][]
        {
            BitConverter.GetBytes(vec.x),
            BitConverter.GetBytes(vec.y),
            BitConverter.GetBytes(vec.z)
        };
        return bytess.Flatten();
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> arrarr)
    { //                                                         DAMN DANIEL ^
        IEnumerable<T> flatted = new List<T>(); // use ienumerable to avoid recasting constantly
        foreach (IEnumerable<T> item in arrarr)
        {
            flatted = flatted.Concat(item);
        }
        return flatted;
    }

    public static byte[] Flatten(this byte[][] arrarr)
    {
        int totalToNow = 0;
        int sum = arrarr.Sum(arr => arr.Length);
        byte[] result = new byte[sum];

        for (int i = 0; i < arrarr.Length; i++)
        {
            Buffer.BlockCopy(arrarr[i], 0, result, totalToNow, arrarr[i].Length);
            totalToNow += arrarr[i].Length;
        }

        return result;
    }

    public static string Join<T>(this IEnumerable<T> seq, string delim = ",")
    {
        return string.Join(delim, seq);
    }

    public static IEnumerable<IEnumerable<T>> SplitList<T>(this IEnumerable<T> source, int maxPerList)
    {
        IList<T> enumerable = source as IList<T> ?? source.ToList();
        if (!enumerable.Any())
        {
            return new List<IEnumerable<T>>();
        }
        return (new List<IEnumerable<T>>() { enumerable.Take(maxPerList) }).Concat(enumerable.Skip(maxPerList).SplitList<T>(maxPerList));
    }

    public static byte[] SerializePosRot(this Transform t)
    {
        byte[] res = new byte[Const.SizeV3 * 2];

        t.position.ToBytes().CopyTo(res, 0);
        t.rotation.eulerAngles.ToBytes().CopyTo(res, Const.SizeV3);

        return res;
    }

    public static void DeserializePosRot(this Transform t, byte[] serializedPosRot, bool dontWarn = false)
    {
#if DEBUG
        if (!dontWarn) if (serializedPosRot.Length != Const.SizeV3 * 2) Chaos.Warn("Deserializing posrot of unexpected length " + serializedPosRot.Length + "!!! This could be bad!!!");
#endif
        Vector3 pos = Utilities.DebyteV3(serializedPosRot);
        Quaternion rot = Quaternion.Euler(Utilities.DebyteV3(serializedPosRot, sizeof(float) * 3));
        t.SetPositionAndRotation(pos, rot);
    }

    public static void PlayClip(this AudioPlayer player, AudioClip clip, float? volume = null)
    {
#if NOBONELIB
        throw new NotImplementedException();
#else
        var vol = new Il2CppSystem.Nullable<float>(volume ?? 0);
        vol.hasValue = volume.HasValue;
        player.Play(clip, player._source.outputAudioMixerGroup, vol, Utilities.NulledNullable<bool>(), Utilities.NulledNullable<float>(), Utilities.NulledNullable<float>(), Utilities.NulledNullable<float>());
#endif
    }

    public static void ForceFireable(this Gun gun)
    {
        // thx swipez https://discord.com/channels/563139253542846474/656631681406468137/1069485703429374092
        //bool magState_inoc = gun.MagazineState is null || gun.MagazineState.WasCollected || gun.MagazineState == null;
        //if (!magState_inoc) gun.MagazineState.Refill();
        //else gun.InstantLoadAsync();

        //gun.CeaseFire();
        //gun.Charge();
        gun.InstantLoadAsync().Forget();
    }
}
