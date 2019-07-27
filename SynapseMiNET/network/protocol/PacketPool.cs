using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{
    class PacketPool
    {

        Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public void init()
        {
            registerPacket(SynapseInfo.CONNECT_PACKET, new ConnectPacket());
            registerPacket(SynapseInfo.BROADCAST_PACKET, new BroadcastPacket());
            registerPacket(SynapseInfo.DISCONNECT_PACKET, new DisconnectPacket());
            registerPacket(SynapseInfo.HEARTBEAT_PACKET, new HeartbeatPacket());
            registerPacket(SynapseInfo.INFORMATION_PACKET, new InformationPacket());
            registerPacket(SynapseInfo.PLAYER_LOGIN_PACKET, new PlayerLoginPacket());
            registerPacket(SynapseInfo.PLAYER_LOGOUT_PACKET, new PlayerLogoutPacket());
            registerPacket(SynapseInfo.PLUGIN_MESSAGE_PACKET, new PluginMessagePacket());
            registerPacket(SynapseInfo.REDIRECT_PACKET, new RedirectPacket());
            registerPacket(SynapseInfo.TRANSFER_PACKET, new TransferPacket());
        }

        public bool ready()
        {
            return packets.Count == 10;
        }

        public void registerPacket(int pid, Packet packet)
        {
            packets.Add(pid, (Packet) packet.Clone());
        }

        public Packet getPacketById(int id, byte[] buffer = null)
        {
            Packet packet = packets[id];   
            if(buffer != null)
            {
                packet.setBuffer(buffer);
                
            }
            return packet;
        }
    }
}
