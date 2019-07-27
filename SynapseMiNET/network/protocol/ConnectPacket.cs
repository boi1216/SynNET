namespace SynapseMiNET.network.protocol
{
    class ConnectPacket : Packet
    {
        public int Id = SynapseInfo.CONNECT_PACKET;

        public byte protocol;
        public int maxPlayers;
        public bool isMainServer;
        public bool isLobbyServer;
        public bool transferShutdown;
        public string identifier;
        public string password;

        public override void encode()
        {
            writeUnsignedVarInt(Id);
            writeInt(protocol);
            writeInt(maxPlayers);
            writeBool(isMainServer);
            writeBool(isLobbyServer);
            writeBool(transferShutdown);
            writeString(identifier);
            writeString(password);
        }

        public override void decode() { }
        
    }
}
