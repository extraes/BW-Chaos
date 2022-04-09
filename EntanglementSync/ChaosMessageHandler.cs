using BWChaos.Effects;
using Discord;
using Entanglement.Modularity;
using Entanglement.Network;
using ModThatIsNotMod;
using System;
using System.Linq;
using System.Text;

namespace BWChaos.Sync
{
    public class ChaosMessageHandler : NetworkMessageHandler
    {
        public override byte? MessageIndex => ChaosSyncHandler.mIndex;

        public override NetworkMessage CreateMessage(NetworkMessageData data)
        {
            if (!(data is ChaosMessageData)) throw new Exception("Unexpected msgdata type");
            var cmd = data as ChaosMessageData;

            byte[] bytes = new byte[][] 
            {
                ChaosSyncHandler.thisVersion,
                new byte[] { (byte)cmd.type, cmd.effectIndex },
                cmd.syncData,
            }.Flatten();

            var msg = new NetworkMessage
            {
                messageType = MessageIndex.Value,
                messageData = bytes,
            };
            return msg;
        }

        // message.data format:
        //  zero based
        //  0-2=version, 3=NetMsgType, 4=idx, 5-end=data
        //  one based
        //  1-3=version, 4=NetMsgType, 5=idx, 6-end=data
        public override void HandleMessage(NetworkMessage message, long sender)
        {
            // Makes sure same bw chaos version
            var msgVer = message.messageData.Take(3);
            if (!msgVer.SequenceEqual(ChaosSyncHandler.thisVersion))
            {
                if (!ChaosSyncHandler.alreadyVersionWarned)
                {
                    ChaosSyncHandler.alreadyVersionWarned = true;

                    DiscordIntegration.userManager.GetUser(sender, (Result res, ref User user) =>
                    {
                        var name = "ID" + sender.ToString();
                        if (res == Result.Ok) name = user.Username;

                        string verMismatch1 = $"BW Chaos version mismatch (you: {string.Join(".", ChaosSyncHandler.thisVersion)}, {name}: {string.Join(".", msgVer)})";
                        string verMismatch2 = $"This causes a mismatch with effect syncing! Do not expect this to function properly!";
                        Notifications.SendNotification(verMismatch1, 8);
                        Notifications.SendNotification(verMismatch2, 5);
                        ModuleLogger.Msg(verMismatch1 + " " + verMismatch2);
                    });
                }
            }

            EffectBase.NetMsgType type = (EffectBase.NetMsgType)message.messageData[3];
            byte idx = message.messageData[4];
            byte[] data = new byte[message.messageData.Length - 5];
            Buffer.BlockCopy(message.messageData, 5, data, 0, data.Length);

            if (Node.isServer)
            {
                if (type == EffectBase.NetMsgType.START)
                {
                    ModuleLogger.Msg("The hell is bozo " + sender + " over there doing trying to start shit? I'm the server!");
                    return;
                }
                else
                {
                    (Node.activeNode as Server).BroadcastMessageExcept(NetworkChannel.Reliable, message.messageData, sender);
                }
                
            }

#if DEBUG
            // DONT CARE ABOUT SENDER LOL BUT LOG IT ANYWAY
            ModuleLogger.Msg(sender.ToString() + " told us " + data);
#endif

            switch (type)
            {
                case EffectBase.NetMsgType.START:
                    var eName = Encoding.ASCII.GetString(data);
                    if (EffectHandler.allEffects.TryGetValue(eName, out EffectBase eObj))
                    {
                        var eToRun = (EffectBase)Activator.CreateInstance(eObj.GetType());
                        eToRun.isNetworked = true;
                        eToRun.Run();
                    }
                    else
                        Utilities.SpawnAd("'" + eName + "' isn't an effect you have, so it won't run, just enjoy the show!");
                    break;

                default:
                    EffectBase._dataRecieved?.Invoke(type, idx, data);
                    break;
            }
        }
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