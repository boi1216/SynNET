using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{
    class InformationPacket : Packet
    {

        public int Id = SynapseInfo.INFORMATION_PACKET;

        public static byte TYPE_LOGIN = 0;
        public static byte TYPE_CLIENT_DATA = 1;
        public static string INFO_LOGIN_SUCCESS = "success";
        public static string INFO_LOGIN_FAILED = "failed";
        public int type;
        public String message;

        public override void encode()
        {
            writeByte((byte)type);
            writeString(message);
        }

        public override void decode()
        {
            type = readByte();
            message = getString();
        }

    }
}
