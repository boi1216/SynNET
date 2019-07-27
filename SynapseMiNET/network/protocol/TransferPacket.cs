using MiNET.Net;
namespace SynapseMiNET.network.protocol
{
    class TransferPacket : Packet
    {

        public UUID uuid;
        public string clientHash;

        public override void encode()
        {
         /*   Write(uuid);
            Write(clientHash);*/
        }

        public override void decode()
        {
         /*   this.uuid = ReadUUID();
            this.clientHash = ReadString();*/
        }
    }
}
