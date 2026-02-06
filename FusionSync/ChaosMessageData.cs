using BLChaos.Effects;
using LabFusion.Data;
using LabFusion.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLChaos.Sync;
internal class ChaosMessageData : IFusionSerializable, IDisposable
{
    public EffectBase.NetMsgType type;
    public byte effectIndex;
    public byte[] syncData = Array.Empty<byte>();

    public void Deserialize(FusionReader reader)
    {
        byte[] readVer = reader.ReadBytes();
        if (!readVer.SequenceEqual(ChaosModule.thisVersion) && ChaosModule.alreadyVersionWarned)
        {
            ChaosModule.Warn($"Version mismatch! Got message from v{string.Join('.', readVer)}, expected {string.Join('.', ChaosModule.thisVersion)}");
            return;
        }


        type = (EffectBase.NetMsgType)reader.ReadByte();
        effectIndex = reader.ReadByte();
        syncData = reader.ReadBytes();
    }

    public void Serialize(FusionWriter writer)
    {
        writer.Write(ChaosModule.thisVersion);
        writer.Write((byte)type);
        writer.Write(effectIndex);
        writer.Write(syncData);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    public static ChaosMessageData Create(EffectBase.NetMsgType type, byte effIdx, byte[] syncData)
    {
        return new ChaosMessageData()
        {
            type = type,
            effectIndex = effIdx,
            syncData = syncData
        };
    }
}

public class ChaosMessage : ModuleMessageHandler
{
    public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
    {
        using FusionReader reader = FusionReader.Create(bytes);
        using ChaosMessageData data = new();
        data.Deserialize(reader);
        
        // we're server. bounce to clients
        if (NetworkInfo.IsServer && isServerHandled)
        {
            // bounce to clients
            using FusionMessage msg = FusionMessage.ModuleCreate<ChaosMessage>(bytes);
            MessageSender.BroadcastMessage(NetworkChannel.Reliable, msg); //todo: not all messages need be reliable.
            return;
        }

        
    }
}
