using System;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    class PlayerControl : AgentControl
    {
        public ClientConnection Client { get; private set; }

        public PlayerControl SetClient(ClientConnection client)
        {
            Client = client; return this;
        }
        
        public override void OnThink(ulong time)
        {
            if (Client == null || !CanMove(time)) return;

            Client.SendVisibleLevelState(Level, time);

            var validKeys = Tools.MovementKeys.Keys
                .Where(x => Entity.CanMove(Tools.MovementKeys[x]))
                .ToArray();

            Move(Tools.MovementKeys[Client.ReadInput(validKeys)], time);

            Client.SendVisibleLevelState(Level, time + 1);
        }
    }
}
