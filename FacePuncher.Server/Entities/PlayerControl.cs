using System;
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Binds control of an entity to a specified client.
    /// </summary>
    class PlayerControl : AgentControl
    {
        /// <summary>
        /// Gets or sets the client that dictates the actions of this entity.
        /// </summary>
        public ClientConnection Client { get; set; }

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
