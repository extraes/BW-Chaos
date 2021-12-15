using BWChaos.Effects;
using Entanglement.Modularity;
using Entanglement.Network;
using System;
using System.Linq;
using System.Text;

namespace BWChaos.Sync
{
    public class ChaosSyncHandler : EntanglementModule
    {
        public static readonly byte mIndex = 99; // 99 because c in binary is 99 :^)
        public static byte[] thisVersion = new byte[3];

        public override void OnModuleLoaded()
        {
            base.OnModuleLoaded();
            ModuleLogger.Msg("Chaos sync module loaded!");

            // Attach version information to each message because I don't want to do handshaking
            string[] versions = BuildInfo.Version.Split('.');
            for (int i = 0; i < thisVersion.Length; i++) thisVersion[i] = byte.Parse(versions[i]);
            // sorry low data enthusiasts, this is an extraes moment

            NetworkMessage.RegisterHandler<ChaosMessageHandler>();

            var ee = new EntangleEffect();
            EffectHandler.AllEffects.Add(ee.Name, ee);

            Chaos.OnEffectRan += OnEffectRan;
            EffectBase._sendData += SendEffectData;
        }
        

        private void OnEffectRan(EffectBase effect)
        {
            if (Node.activeNode.connectedUsers.Count == 0) return;

            if (!IsEffectSyncable(effect.Types))
            {
                ModuleLogger.Msg("Not going to sync " + effect.Name);
                Utilities.SpawnAd($"Not gonna sync this effect lol:\n{effect.Name}");
                return;
            }
            
            // send data
            var msg = NetworkMessage.CreateMessage(mIndex, new ChaosMessageData { effectName = effect.Name, syncData = "start" });
            ModuleLogger.Msg("Telling Entanglement to sync effect: " + effect.Name);
            Node.activeNode.BroadcastMessage(NetworkChannel.Reliable, msg.GetBytes());
        }

        private bool IsEffectSyncable(EffectBase.EffectTypes types)
        {
            return !(types.HasFlag(EffectBase.EffectTypes.USE_STEAM) || types.HasFlag(EffectBase.EffectTypes.AFFECT_STEAM_PROFILE) || types.HasFlag(EffectBase.EffectTypes.DONT_SYNC));
        }


        private static void SendEffectData(string name, string data)
        {
            if (Node.activeNode.connectedUsers.Count == 0) return;
            var msg = NetworkMessage.CreateMessage(mIndex, new ChaosMessageData { effectName = name, syncData = data });
            Node.activeNode.BroadcastMessage(NetworkChannel.Reliable, msg.GetBytes());
        }
    }

    public class ChaosMessageHandler : NetworkMessageHandler
    {
        public override byte? MessageIndex { get; } = ChaosSyncHandler.mIndex;

        public override NetworkMessage CreateMessage(NetworkMessageData data)
        {
            if (!(data is ChaosMessageData)) throw new Exception("Unexpected msgdata type");
            var cmd = data as ChaosMessageData;

            byte[] bytes = ChaosSyncHandler.thisVersion
                           .Concat(Encoding.UTF8.GetBytes(cmd.effectName+ ":" + cmd.syncData)).ToArray();

            var msg = new NetworkMessage
            {
                messageType = MessageIndex.Value,
                messageData = bytes,
            };
            return msg;
        }

        public override void HandleMessage(NetworkMessage message, long sender)
        {
            if (Node.isServer)
            {
                ModuleLogger.Msg("The hell is bozo " + sender + " over there doing trying to order me around? I'm the server!");
                return;
            }
            // Makes sure same bw chaos version
            var msgVer = message.messageData.Take(3);
            if (!msgVer.SequenceEqual(ChaosSyncHandler.thisVersion))
                ModuleLogger.Msg("BW Chaos version mismatch!!! Expected: " + string.Join(",", ChaosSyncHandler.thisVersion) + ". Got: " + string.Join(",", msgVer));


            string strData = Encoding.UTF8.GetString(message.messageData.Skip(3).ToArray());
            string[] args = Utilities.Argsify(strData, ':'); // [name, data]
#if DEBUG
            // DONT CARE ABOUT SENDER LOL BUT LOG IT ANYWAY
            ModuleLogger.Msg(sender.ToString() + " told us " + strData);
#endif

            switch (args[1])
            {
                case "start":
                    if (EffectHandler.AllEffects.TryGetValue(args[0], out EffectBase eObj))
                    {
                        var eToRun = (EffectBase)Activator.CreateInstance(eObj.GetType());
                        eToRun.isNetworked = true;
                        eToRun.Run();
                    }
                    else
                        Utilities.SpawnAd("'" + args[0] + "' isn't an effect you have, so it won't run, just enjoy the show!");
                    break;

                default:
                    EffectBase._dataRecieved?.Invoke(args[0], args[1]);
                    break;
            }
        }
    }
    public class ChaosMessageData : NetworkMessageData
    {
        public string effectName;
        public string syncData;
    }

    //public class ChaosMessageData : INetworkSerializable
    //{
    //    public string effectName;
    //    public byte[] version;
    //    public void FromBytes(byte[] bytes)
    //    {
    //        version = bytes.Take(3).ToArray();
    //        effectName = Encoding.UTF8.GetString(bytes.Skip(3).ToArray());
    //    }

    //    public uint GetSize()
    //    {
    //        return (uint)Encoding.UTF8.GetByteCount(effectName) + 3;
    //    }

    //    public byte[] ToBytes()
    //    {
    //        byte[] bytes = ChaosSyncHandler.thisVersion;
    //        bytes = bytes.Concat(Encoding.UTF8.GetBytes(effectName)).ToArray();
    //        return bytes;
    //    }
    //}
}




/*using BWChaos.Effects;
using Entanglement.Objects;
using Entanglement.Modularity;
using Entanglement.Network;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using MelonLoader;
using Entanglement.Extensions;

namespace BWChaos.Sync
{
    public class ChaosSyncHandler : EntanglementModule
    {
        public static Action<string> OnMessageRecieved;
        static byte mIndex = 99;
        public static byte[] thisVersion = new byte[3];

        public override void OnModuleLoaded()
        {
            ModuleLogger.Msg("Chaos sync module loaded!");
            // Attach version information to each message because I don't want to do handshaking
            string[] versions = BuildInfo.Version.Split('.');
            for (int i = 0; i < thisVersion.Length; i++) thisVersion[i] = byte.Parse(versions[i]);
            // sorry low data enthusiasts, this is an extraes moment

            Net.RegisterHandler<ChaosMessageHandler>();

            BWChaos.OnEffectRan += OnEffectRan;
        }

        private void OnEffectRan(EffectBase effect)
        {
            if (effect.Types.HasFlag(EffectBase.EffectTypes.USE_STEAM) || effect.Types.HasFlag(EffectBase.EffectTypes.AFFECT_STEAM_PROFILE) || effect.Types.HasFlag(EffectBase.EffectTypes.DONT_SYNC))
            {
                ModuleLogger.Warn("Not going to sync " + effect.Name);
                return; // bad. very bad.
            }
            if (!Node.isServer)
            {
                return; // Don't let clients dictate shit to the server, very bad idea
            }
            // send data
            using (var msg = new NetworkMessage(NetworkChannel.Reliable))
            {
                msg.messageType = mIndex;
                msg.messageData = thisVersion;
                msg.messageData = msg.messageData.Concat(Encoding.UTF8.GetBytes(effect.Name)).ToArray();
                
            }
        }

        
    }

    public static class Extensions
    {
        public static void OwnObject(this GameObject go)
        {
            //HOPEFULLY this works
            if (go.IsBlacklisted() || !Node.isServer) return;
            using (var msg = new NetworkMessage(NetworkChannel.Object))
            {
                ObjectSync.GetPooleeData(go.transform, out Rigidbody[] rbs, out string rootname, out short spindex, out float sptime);
                foreach (var rb in rbs) SyncUtilities.UpdateBodyDetached(rb);
            }
        }

        public static void UnownObject(this GameObject go)
        {
            //HOPEFULLY this works
            if (go.IsBlacklisted() || !Node.isServer) return;
            using (var msg = new NetworkMessage(NetworkChannel.Object))
            {
                ObjectSync.GetPooleeData(go.transform, out Rigidbody[] rbs, out string rootname, out short spindex, out float sptime);
                foreach (var rb in rbs) SyncUtilities.UpdateBodyAttached(rb, rootname, spindex, sptime);
            }
        }
    }

    public class ChaosMessageHandler : NetworkMessageHandler
    {
        public override byte? MessageIndex { get; } = 99; // 99 because c in binary is 99 :^)

        public override void HandleMessage(NetworkMessage message, long sender)
        {
            if (Node.isServer) {
                ModuleLogger.Warn("The hell is bozo " + sender + " over there doing trying to order me around? I'm the server!");
                return;
            }
            // Makes sure same bw chaos version
            var msgVer = message.messageData.Take(3);
            if (!msgVer.SequenceEqual(ChaosSyncHandler.thisVersion)) 
                ModuleLogger.Warn("BW Chaos version mismatch!!! Expected: " + string.Join(",", ChaosSyncHandler.thisVersion) + ". Got: " + string.Join(",", msgVer));
            

            // DONT CARE ABOUT SENDER LOL BUT LOG IT ANYWAY
            string strData = Encoding.UTF8.GetString(message.messageData.Skip(3).ToArray());
#if DEBUG
            ModuleLogger.Msg(sender.ToString() + " told us " + strData);
#endif
            EffectHandler.AllEffects.TryGetValue(strData, out EffectBase eToRun);
            if (eToRun != null)
                eToRun.Run();
            else
            {
                Utilities.SpawnAd("'" + strData + "' isn't an effect you have, so it won't run, just enjoy the show!");
            }
        }
    }

    public class ChaosMessageData : INetworkSerializable
    {
        public string effectName;
        public byte[] version;
        public void FromBytes(byte[] bytes)
        {
            version = bytes.Take(3).ToArray();
            effectName = Encoding.UTF8.GetString(bytes.Skip(3).ToArray());
        }

        public uint GetSize()
        {
            return (uint)Encoding.UTF8.GetByteCount(effectName) + 3;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = ChaosSyncHandler.thisVersion;
            bytes = bytes.Concat(Encoding.UTF8.GetBytes(effectName)).ToArray();
            return bytes;
        }
    }
}
*/