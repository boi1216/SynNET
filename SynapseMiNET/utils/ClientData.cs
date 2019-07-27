using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.utils
{
    class ClientData
    {

        public Dictionary<string, Entry> clientHash = new Dictionary<string, Entry>();

        public string getHashByIdentifier(string identifier)
        {
            string[] re = new string[1];

            foreach(KeyValuePair<string, Entry> client in clientHash)
            {
                if(client.Value.getIdentifier() == identifier)
                {
                    re[0] = client.Key;
                }
            }
            return re[0];
        }


    }

    class Entry
    {
        private string ip;
        private int port;
        private int playerCount;
        private int maxPlayers;
        private string identifier;

        public string getIp()
        {
            return ip;
        }

        public int getPort()
        {
            return port;
        }

        public int getPlayerCount()
        {
            return playerCount;
        }

        public int getMaxPlayers()
        {
            return maxPlayers;
        }

        public string getIdentifier()
        {
            return identifier;
        }

    }
}
