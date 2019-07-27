using MiNET.Net;

namespace SynapseMiNET.network.protocol
{
    class RedirectPacket : Packet
    {

        public UUID uuid;
        public bool direct;
        public byte[] mcpeBuffer;

        public override void encode()
        {
            writeUUID(uuid);
            writeBool(direct);
            writeUnsignedVarInt(mcpeBuffer.Length);
            writeByteArray(mcpeBuffer);
        }

        public override void decode()
        {
            this.uuid = readUUID();
            this.direct = readBool();
            this.mcpeBuffer = readByteArray((int)getUnsignedVarInt());
        }

        
    }
}
