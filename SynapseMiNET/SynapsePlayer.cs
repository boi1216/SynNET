using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using MiNET;

namespace SynapseMiNET
{
    class SynapsePlayer : Player
    {

        private IPEndPoint realEndpoint;

        public SynapsePlayer(MiNetServer server, IPEndPoint endpoint) : base(server, endpoint)
        {

        }

       

        public IPEndPoint getRealAddress()
        {
            return realEndpoint;
        }
    }
}
