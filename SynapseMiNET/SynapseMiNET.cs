using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using SynapseMiNET.network.protocol;
using SynapseMiNET.utils;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET;
using MiNET.Utils;

namespace SynapseMiNET
{
    [Plugin(PluginName = "SynapseMiNET", Description = "Synapse client for MiNET with Nemisys protocol support", PluginVersion = "1.0", Author = "boi")]
    public class Class1 : Plugin
    {

        public IPEndPoint NemisysEndpoint { get; private set; }

        public Socket _client;
        public MiNetServer server;
        

        private bool loggedIn = false;

        TimeSpan lastConnectionAttempt = new TimeSpan();
        int lastUpdate;
        int startTime;
        PacketPool packetPool = new PacketPool();
        AsyncTicker asyncTickerThread;

        static Queue<byte[]> internalPacketQueue = new Queue<byte[]>();

        private SynapseMiNET.utils.Config _config;

        ClientData clientData;
        private byte[] receiveBuffer = new byte[65535];

        private string password;
        private string address;
        private int port;
        private string identifier;
        private bool isMainServer;
        private bool isLobbyServer;
        private bool transferOnShutdown;

        protected override void OnEnable()
        {
            server = Context.Server;
            lastUpdate = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            startTime = lastUpdate;
            try
            {
                _config = new SynapseMiNET.utils.Config();
            }catch(ConfigValidationException e)
            {
                Message("Failed to load config");
                return;
            }


            password = _config.getValue("password");
            address = _config.getValue("address");
            port = Int32.Parse(_config.getValue("port"));
            identifier = _config.getValue("identifier");
            isMainServer = bool.Parse(_config.getValue("isMainServer"));
            isLobbyServer = bool.Parse(_config.getValue("isLobbyServer"));
            transferOnShutdown = bool.Parse(_config.getValue("transferOnShutdown"));

            Message("Address= " + address + " Port=" + port);

            createListener();
            packetPool.init();

            if (packetPool.ready())
            {
                asyncTickerThread = new AsyncTicker(this);
                asyncTickerThread.Start();
            }
        }

        public void processPacket(int pid, byte[] buffer)
        {
            Packet packet = packetPool.getPacketById(pid, buffer);
        }

        
        public static void Message(string message)
        {
            Console.WriteLine("[SynapseClient] " + message);
        }

        public void createListener()
        {
            this._client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           
            try
            {
                _client.Connect(address, port);
            }
            catch (Exception e)
            {
                Message("Failed to initialize connection with " + address + ":" + port);
                Message("Reason: " + e.Message);
                lastConnectionAttempt = DateTime.Now.TimeOfDay;
                return;
            }

            _client.NoDelay = true;
            _client.ReceiveBufferSize = int.MaxValue;
            _client.SendBufferSize = int.MaxValue;

            Message("Connection initialized with " + address + ":" + port);

            new Thread(processNetwork) { IsBackground = true }.Start();
            this.connect();
        }

        private void sendDataPacket(Packet packet, int pid)
        {
            byte[] packetBuffer = packet.getEncoded();

            string hex = SynapseInfo.PROTOCOL_MAGIC.ToString("X").ToLower();
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
               bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            _client.Send(bytes);
            string hexID = "0" + pid.ToString("X");
            byte[] bytesID = new byte[hexID.Length / 2];
            for (int i = 0; i < hexID.Length; i += 2)
            {
                bytesID[i / 2] = Convert.ToByte(hexID.Substring(i, 2), 16);
            }
            _client.Send(bytesID);

            byte[] lenBytes = BitConverter.GetBytes((Int32)(packetBuffer.Length - 1));
            Array.Reverse(lenBytes);
            _client.Send(lenBytes);

            packetBuffer.Skip(1);
            _client.Send(packetBuffer);

        }

        
        public void processNetwork()
        {
            byte[] recvBytes = new byte[6555];


            while (_client.Connected)
            {
                _client.Receive(recvBytes);
            
          
               internalPacketQueue.Enqueue(recvBytes);
            }
        }

        protected void connect()
        {
            ConnectPacket packet = new ConnectPacket();
            packet.protocol = SynapseInfo.CURRENT_PROTOCOL;
            packet.maxPlayers = 5000;
            packet.isLobbyServer = this.isLobbyServer;
            packet.isMainServer = this.isMainServer;
            packet.identifier = this.identifier;
            packet.password = this.password;
            packet.transferShutdown = this.transferOnShutdown;

            packet.encode();
            this.sendDataPacket(packet, packet.Id);
        }

        protected void disconnect(string reason = "Server Closed")
        {
            if (this.loggedIn)
            {
                DisconnectPacket packet = new DisconnectPacket();
                packet.type = DisconnectPacket.TYPE_GENERIC;
                packet.message = reason;
                packet.encode();

                _client.Send(packet.getEncoded());
            }
        }

        protected void reconnect()
        {
            disconnect();
            connect();
        }

        private void handlePacket(int pid, Packet packet)
        {
            switch (pid)
            {
                case SynapseInfo.DISCONNECT_PACKET:
                    DisconnectPacket disconnectPacket = (DisconnectPacket)packet;
                    disconnectPacket.decode();
                    this.loggedIn = false;

                    Message("Disconnect received: " + disconnectPacket.message);

                    break;
                case SynapseInfo.INFORMATION_PACKET:
                    InformationPacket packetInformation = (InformationPacket)packet;
                    packetInformation.decode();
                    
                    if (packetInformation.type == InformationPacket.TYPE_LOGIN)
                    {
                        if (packetInformation.message == InformationPacket.INFO_LOGIN_SUCCESS)
                        {
                            this.loggedIn = true;
                            Message("Login sucess to " + address + ":" + port);
                        }
                        else
                        {
                            Message("Login failed to " + address + ":" + port);
                        }
                    }
                    else if (packetInformation.type == InformationPacket.TYPE_CLIENT_DATA)
                    {
                       var clientHash = Newtonsoft.Json.JsonConvert.DeserializeObject(packetInformation.message);
                        //TODO: pass into ClientData object
                    }
                    
                    break;
                case SynapseInfo.PLAYER_LOGIN_PACKET:
                    PlayerLoginPacket packetLogin = (PlayerLoginPacket)packet;
                    packetLogin.decode();
                    Message("Player joining...");
                  //  Message("Player UUID: " + packetLogin.uuid.ToString());
                    Message("Player address: " + packetLogin.address);
                   
                    break;
            }
        }

        public void process() {
            readPacket();
        }

        public void readPacket()
        {
            if (internalPacketQueue.Count > 0)
            {
                int offset = 0;
                byte[] buffer = internalPacketQueue.Dequeue();
                int len = buffer.Length;
                a:
                byte[] magicArray = buffer.Take(2).ToArray();
                int MAGIC = readShort(magicArray);
                if (MAGIC != SynapseInfo.PROTOCOL_MAGIC)
                {
                    return;
                }
                int pid = buffer[offset + 2];

                byte[] lengthArray = buffer.Skip(3).Take(4).ToArray();
                int packetLength = readInt(lengthArray);

                offset += 7;

                byte[] packetBuffer = buffer.Skip(offset).Take(packetLength).ToArray();

                if (packetLength <= (len - offset))
                {
                    Packet packet = packetPool.getPacketById(pid, packetBuffer);
                    offset += packetLength;
                    if (packet != null)
                    {
                        handlePacket(pid, packet);
                        return;
                    }
                }
                else
                {
                    offset -= 7;
                    buffer = buffer.Skip(offset).ToArray();
                    len = buffer.Length;
                    goto a;
                }
            }
        }

        public static int readInt(byte[] bytes)
        {
            return ((bytes[0] & 0xff) << 24) +
                    ((bytes[1] & 0xff) << 16) +
                    ((bytes[2] & 0xff) << 8) +
                    (bytes[3] & 0xff);
        }

        public static int readShort(byte[] bytes)
        {
            return ((bytes[0] & 0xFF) << 8) + (bytes[1] & 0xFF);
        }


        public void threadTick()
        {
            this.process();
            if (!this._client.Connected) return;

            int currentTime = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if((currentTime - lastUpdate) >= 5000)
            {
                lastUpdate = currentTime;
                HeartbeatPacket packet = new HeartbeatPacket();
                packet.tps = 20; //Calculation needed
                packet.load = 20; //-||-
                packet.upTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
                packet.encode();
                sendDataPacket(packet, packet.Id);
            }

            int finalTime = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if((finalTime - currentTime) >= 30000){
                reconnect();
          
            }
        }

        public override void OnDisable()
        {
            disconnect();
        }

    }

    public class AsyncTicker
    {

       
        public Thread asyncTicker;
        private long lastWarning = 0;
        private long tickUseTime;
        private Class1 plugin;

        public AsyncTicker(Class1 plugin)
        {
            this.plugin = plugin;
        }

        public void Start()
        {
            asyncTicker = new Thread(new ThreadStart(Run));
            asyncTicker.Start();
        }

        public void Run()
        {

            while (true)
            {
                plugin.threadTick();
            }
        }

        public double getTicksPerSecond()
        {
            long more = tickUseTime - 10;
            if (more < 0) return 100;
            return Math.Round(10f / (double) tickUseTime, 3) * 100;
        }

    }

}
