using System;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    class PlayerControl : Component
    {
        public Client Client { get; private set; }

        public PlayerControl SetClient(Client client)
        {
            Client = client; return this;
        }

        public override void OnThink(ulong time)
        {
            if (Client == null) return;

            var direc = Tools.MovementKeys[Client.ReadInput(Tools.MovementKeys.Keys.ToArray())];
            
            Entity.Move(Tile.GetNeighbour(direc));
        }
    }
}
