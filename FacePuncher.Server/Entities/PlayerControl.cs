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

                var validKeys = Tools.MovementKeys.Keys
                    .Where(x => Entity.CanMove(Tools.MovementKeys[x]))
                    .ToArray();

                Entity.Move(Tools.MovementKeys[Client.ReadInput(validKeys)]);

                Client.SendVisibleLevelState(Level, time + 1);
            }
        }
    }
}
