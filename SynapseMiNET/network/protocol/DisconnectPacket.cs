using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{
    class DisconnectPacket : Packet
    {

        public static int NETWORK_ID = SynapseInfo.DISCONNECT_PACKET;

        public static byte TYPE_WRONG_PROTOCOL = 0;
        public static byte TYPE_GENERIC = 1;
        public byte type;
        public string message;

        public override void encode()
        {
        /*    Write(this.type);
            Write(this.message);*/
            
        }

        public override void decode()
        {
          /*  this.type = this.ReadByte();
            this.message = this.ReadString();*/
        }
    }
}
