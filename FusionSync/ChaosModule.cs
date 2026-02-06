namespace BLChaos.Sync;

using BLChaos.Effects;
using Jevil;
using LabFusion.Extensions;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.Representation;
using LabFusion.SDK.Modules;
using LabFusion.Utilities;
using System.Text;

public static class BuildInfo
{
    public const string Name = "ChaosSync"; // Name of the Module.  (MUST BE SET)
    public const string Version = BLChaos.BuildInfo.Version; // Version of the Module.  (MUST BE SET)
    public const string Author = "extraes"; // Author of the Module.  (MUST BE SET)
    public const string Abbreviation = "Chaos"; // Abbreviation of the Module. (Set as null if none)
    public const bool AutoRegister = true; // Should the Module auto register when the assembly is loaded?
    public const ConsoleColor Color = ConsoleColor.Yellow; // The color of the logged load info. (MUST BE SET)
}

public class ChaosModule : Module
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static ChaosModule Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable
    public ChaosModule() => Instance = this;
    public static bool IsAlone => PlayerIdManager.PlayerCount <= 1;
    public static byte[] thisVersion = new byte[3];
    public static bool alreadyVersionWarned = false;
    public static Encoding TextEncoding = Encoding.UTF8;

    public override void OnModuleLoaded()
    {
        // Attach version information to each message because I don't want to do handshaking
        string[] versions = BuildInfo.Version.Split('.');
        for (int i = 0; i < thisVersion.Length; i++) thisVersion[i] = byte.Parse(versions[i]);
        // sorry low data enthusiasts, this is an extraes moment

        //Chaos.InjectEffect<EntangleEffect>();
        //Chaos.InjectEffect<PlayerRepresenting>();
        //Chaos.InjectEffect<EntanglementJoins>();

        MultiplayerHooking.OnJoinServer += () => alreadyVersionWarned = false;
        Chaos.OnEffectRan += OnEffectRan;
        EffectBase._sendData += SendEffectData;
    }


    private void OnEffectRan(EffectBase effect)
    {
        // playercount includes host
        
        if (IsAlone && !NetworkInfo.IsServer) return;
        if (effect.isNetworked) return;

        if (!IsEffectSyncable(effect.Types))
        {
            LoggerInstance.Log("Not going to sync " + effect.Name);
            Utilities.SpawnAd($"Not gonna sync this effect lol:\n{effect.Name}");
            return;
        }

        // send data
        using FusionWriter writer = new();
        // starting an effect doesnt need an index cause the effect is found via its name
        using ChaosMessageData msg = ChaosMessageData.Create(EffectBase.NetMsgType.START, 0, TextEncoding.GetBytes(effect.Name));
        writer.Write(msg);
        using FusionMessage fm = FusionMessage.ModuleCreate<ChaosMessage>(writer);
        Log("Telling Entanglement to sync effect: " + effect.Name);
        MessageSender.BroadcastMessage(NetworkChannel.Reliable, fm);
    }

    private bool IsEffectSyncable(EffectBase.EffectTypes types)
    {
        return !(types.HasFlag(EffectBase.EffectTypes.USE_STEAM) || types.HasFlag(EffectBase.EffectTypes.AFFECT_STEAM_PROFILE) || types.HasFlag(EffectBase.EffectTypes.DONT_SYNC));
    }


    private static void SendEffectData(EffectBase.NetMsgType msgType, byte index, byte[] data)
    {
        using FusionWriter writer = new();
        using ChaosMessageData msg = ChaosMessageData.Create(msgType, index, data);
        writer.Write(msg);
        using FusionMessage fm = FusionMessage.ModuleCreate<ChaosMessage>(writer);
        MessageSender.BroadcastMessage(NetworkChannel.Reliable, fm);
    }

    public static void Log(string message) => Instance.LoggerInstance.Log(message);
    public static void Warn(string message) => Instance.LoggerInstance.Warn(message);
}
