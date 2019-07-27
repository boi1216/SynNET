using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{

    class PluginMessagePacket : Packet
    {

        public string channel;
        public byte[] data;

        public override void encode()
        {
          /*  Write(channel);
            Write(data);*/
        }

        public override void decode()
        {
          /*  this.channel = ReadString();
            this.data = ReadByteArray();*/
        }
    }
}
