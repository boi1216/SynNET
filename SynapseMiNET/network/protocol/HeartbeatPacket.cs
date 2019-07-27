using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.network.protocol
{
    class HeartbeatPacket : Packet
    {

        public int Id = SynapseInfo.HEARTBEAT_PACKET;

        public float tps;
        public float load;
        public long upTime;


        public override void encode()
        {
            writeId();
            writeFloat(tps);
            writeFloat(load);
            writeLong(upTime);
        }

        public override void decode() { }

    }
}
