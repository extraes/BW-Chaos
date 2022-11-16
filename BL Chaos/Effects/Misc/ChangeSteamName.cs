using Steamworks;
using Steamworks.Data;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BLChaos.Effects;

internal class ChangeSteamName : EffectBase
{
    // Shortened to 2m to avoid the effect running twice
    public ChangeSteamName() : base("Change Steam name for 2m", 120, EffectTypes.USE_STEAM | EffectTypes.AFFECT_STEAM_PROFILE | EffectTypes.DONT_SYNC) { }

    private static readonly string[] names = new string[] {
        "Chester Chucklenuts",
        "Your little PogChamp",
        "✨ 💓💞❤️ Deku X Bakugo ❤️💞💓 ✨",
        "Sydney|14|Bi|They/Them|BLM|ACAB",
        "lars|gay|transmasc|allosexual/poly|libra|ravenclaw",
        "xXx_DD0S_H4XX_xXx",
        "Persnickety",
        "4K African",
        "Brayden|32|ladies man|4'3\"|short kings stay winning",
        "Brylan the wolf owo",
        "Brylan Bristopher|CEO|LLC Owner|$DOGE HODLer🚀|Multimillionaire|Bossman, ❌suit ❌tie",
        "xXAn0nym0usXx",
        "shoutouts to simpleflips",
        "Based, Redpilled | Dobe Johnson",
        "Hack Dudes 69",
        "Lawrence Albert Connor",
        "Big. Black. River balls.", // i dont have the pass :woeis:
        "The Holy Thighble",
        "#CryptoPunks TAP IN!!!",
        "astolfo enjoyer",
        "cs.money|darren.chungus.09",
        "edgelord mcspice",
        "'HATRED' GOTY 2015",
        "isometric sexercise",
        "i am so god damn racist i hate n",
        "heap overflow causer",
        "anime women gooner",
        "lubriderm's top customer",
        "BL Light Avatar R34 fan",
    };

    private string steamName = "helo :)";
    public override void OnEffectStart()
    {
        try
        {
            steamName = Steamworks.SteamClient.Name;
            string newName = $"{names.Random()}-BLC"; // yeah thisll definitely get cut off by how long most of the names are. oh well lol
            SetPersonaName(newName);
            Utilities.SpawnAd("So, how goes it, " + newName + "?");
        }
        catch (Exception ex)
        {
            Chaos.Warn("Changing steam username failed - " + ex.ToString());
        }
    }

    public override void OnEffectEnd()
    {
        try
        {
            SetPersonaName(steamName);
        }
        catch (Exception ex)
        {
            Chaos.Warn("SetPersonaName call failed - " + ex.ToString());
        }
    }

    // this is so fucked lmfao
    // todo: see how this reacts on oculus builds, or if it even works
    #region Manual method unstripping!
    #region FunctionMeta
    [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_SetPersonaName", CallingConvention = CallingConvention.Cdecl)]
    private static extern SteamAPICall_t _SetPersonaName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringToNative))] string pchPersonaName);

    #endregion
    internal void SetPersonaName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringToNative))] string pchPersonaName)
    {
        // i literally do not give a rat's fuck about the return value, so im just gonna comment this stuff out to remove any extra error surface
        /*SteamAPICall_t returnValue = */_SetPersonaName(SteamFriends.Internal.Self, pchPersonaName);
        //return new CallResult<SetPersonaNameResponse_t>(returnValue, SteamFriends.Internal.IsServer);
        
    }

    // taken from Facepunch.Steamworks source code, in case the runtime doesn't like the IL2CPP-ified Utf8StringToNative
    internal unsafe class Utf8StringToNative : ICustomMarshaler
    {
        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (managedObj == null)
                return IntPtr.Zero;

            if (managedObj is string str)
            {
                fixed (char* strPtr = str)
                {
                    int len = Encoding.UTF8.GetByteCount(str);
                    var mem = Marshal.AllocHGlobal(len + 1);

                    var wlen = System.Text.Encoding.UTF8.GetBytes(strPtr, str.Length, (byte*)mem, len + 1);

                    ((byte*)mem)[wlen] = 0;

                    return mem;
                }
            }

            return IntPtr.Zero;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData) => throw new System.NotImplementedException();
        public void CleanUpNativeData(IntPtr pNativeData) => Marshal.FreeHGlobal(pNativeData);
        public void CleanUpManagedData(object managedObj) => throw new System.NotImplementedException();
        public int GetNativeDataSize() => -1;

        // [Preserve]
        public static ICustomMarshaler GetInstance(string cookie) => new Utf8StringToNative();
    }

    internal struct Utf8StringPointer
    {
#pragma warning disable 649
        internal IntPtr ptr;
#pragma warning restore 649

        public unsafe static implicit operator string(Utf8StringPointer p)
        {
            if (p.ptr == IntPtr.Zero)
                return null;

            var bytes = (byte*)p.ptr;

            var dataLen = 0;
            while (dataLen < 1024 * 1024 * 64)
            {
                if (bytes[dataLen] == 0)
                    break;

                dataLen++;
            }

            return Encoding.UTF8.GetString(bytes, dataLen);
        }
    }
    #endregion
}
