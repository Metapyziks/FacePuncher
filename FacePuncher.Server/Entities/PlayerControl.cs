using System;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    class PlayerControl : Component
    {
        private ulong _nextMovement;

        public Client Client { get; private set; }

        public ulong MovementPeriod
        {
            get { return 100; }
        }

        public PlayerControl SetClient(Client client)
        {
            Client = client; return this;
        }

        public override void OnThink(ulong time)
        {
            if (Client == null) return;

            if (time >= _nextMovement) {
                _nextMovement += MovementPeriod;

                Client.SendVisibleLevelState(Level, time);

                var direc = Tools.MovementKeys[Client.ReadInput(
                    Tools.MovementKeys.Keys.Where(x =>
                        Entity.CanMove(Tile.GetNeighbour(Tools.MovementKeys[x])))
                    .ToArray())];

                Entity.Move(Tile.GetNeighbour(direc));

                Client.SendVisibleLevelState(Level, time + 1);
            }
        }
    }
}
