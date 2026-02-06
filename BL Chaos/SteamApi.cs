using Il2CppSteamworks;
using Il2CppSteamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BLChaos;

// none of these should return a value from Steamworks or Facepunch.Steamworks, because that'd make the methods hard-depend on them instead of being able to just be used from an if statement
internal static class SteamApi
{
    public static string Name
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        get { return SteamClient.Name; }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void SetPersonaName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Jevil.Unsafe.FacepunchUtf8Converter))] string pchPersonaName)
    {
        // i literally do not give a rat's fuck about the return value, so im just gonna comment this stuff out to remove any extra error surface
        /*SteamAPICall_t returnValue = */
        SteamApiImpl.SetPersonaName(SteamFriends.Internal.Self, pchPersonaName);
        //return new CallResult<SetPersonaNameResponse_t>(returnValue, SteamFriends.Internal.IsServer);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ActivateOverlayToWebPage(string str, bool openModal = false)
    {
        const int OPEN_DEFAULT = 0;
        const int OPEN_MODAL = 1;
        SteamApiImpl.ActivateGameOverlayToWebPage(SteamFriends.Internal.Self, str, openModal ? OPEN_MODAL : OPEN_DEFAULT);
    }
}


file static class SteamApiImpl
{
    [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_SetPersonaName", CallingConvention = CallingConvention.Cdecl)]
    internal static extern SteamAPICall_t SetPersonaName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Jevil.Unsafe.FacepunchUtf8Converter))] string pchPersonaName);

    [DllImport("steam_api64", EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ActivateGameOverlayToWebPage(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Jevil.Unsafe.FacepunchUtf8Converter))] string pchURL, int eMode);

    
}