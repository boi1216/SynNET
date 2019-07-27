using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{
    public class SynapseInfo
    {

        public static byte CURRENT_PROTOCOL = 10;
        public const int PROTOCOL_MAGIC = 0xbabe;

        public const byte HEARTBEAT_PACKET = 0x01;
        public const int CONNECT_PACKET = 0x02;
        public const byte DISCONNECT_PACKET = 0x03;
        public const byte REDIRECT_PACKET = 0x04;
        public const byte PLAYER_LOGIN_PACKET = 0x05;
        public const byte PLAYER_LOGOUT_PACKET = 0x06;
        public const byte INFORMATION_PACKET = 0x07;
        public const byte TRANSFER_PACKET = 0x08;
        public const byte BROADCAST_PACKET = 0x09;
        public const byte PLUGIN_MESSAGE_PACKET = 0x0a;
    }
}
