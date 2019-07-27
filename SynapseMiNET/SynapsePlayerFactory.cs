using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MiNET.Utils;
using MiNET;

namespace SynapseMiNET
{
    class SynapsePlayerFactory : PlayerFactory
    {

        public override Player CreatePlayer(MiNetServer server, IPEndPoint endPoint, PlayerInfo playerInfo)
        {
            var player = new SynapsePlayer(server, endPoint);
            player.MaxViewDistance = Config.GetProperty("MaxViewDistance", 22);
            player.MoveRenderDistance = Config.GetProperty("MoveRenderDistance", 1);
            OnPlayerCreated(new PlayerEventArgs(player));
            return  player;
        }


    }
}
