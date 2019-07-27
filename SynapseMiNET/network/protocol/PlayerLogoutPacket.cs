using MiNET.Net;

namespace SynapseMiNET.network.protocol
{
    class PlayerLogoutPacket : Packet
    {

        public UUID uuid;
        public string reason;

        public override void encode()
        {
         /*   Write(uuid);
            Write(reason);*/
        }

        public override void decode()
        {
          /*  this.uuid = ReadUUID();
            this.reason = ReadString();*/
        }
    }
}
