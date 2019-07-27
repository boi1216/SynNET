using MiNET.Net;

namespace SynapseMiNET.network.protocol
{
    class PlayerLoginPacket : Packet
    {

        public UUID uuid;
        public string address;
        public int port;
        public bool isFirstTime;
        public byte[] cachedLogin;

        public override void encode()
        {
            writeId();
            writeUUID(uuid);
            writeString(address);
            writeInt(port);
            writeBool(isFirstTime);
            writeInt(cachedLogin.Length);
            writeByteArray(cachedLogin);
        }

        public override void decode()
        {
            _buffer.Position = 16;
          //  uuid = readUUID();
            address = getString(); //TODO: rename to readString()
            port = readInt();
            isFirstTime = readBool();
            cachedLogin = readByteArray(readInt());
        }
    }
}
