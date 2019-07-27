using System;
using System.Collections.Generic;
using System.Text;
using MiNET.Net;

namespace SynapseMiNET.network.protocol
{
    class BroadcastPacket : Packet
    {
        public List<UUID> entries;
        public bool direct;
        public byte[] payload;

        public override void encode()
        {
         /*   Write(direct);
            Write((short) entries.Count);
            foreach(UUID entry in this.entries)
            {
                Write(entry);
            }

            Write((short) payload.Length);
            Write(payload);*/

        }

        public override void decode()
        {
          /*  this.direct = this.ReadBool();
            int len = this.ReadShort();

            for (int i = 0; i < len; i++)
            {
                this.entries.Add(this.ReadUUID());
            }

            this.payload = this.ReadBytes(this.ReadShort());*/
        }
    }
}
