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

        public override void OnThink()
        {
            if (Client == null || !CanMove) return;

            Client.SendVisibleLevelState();

            var validKeys = Tools.MovementKeys.Keys
                .Where(x => Entity.CanMove(Tools.MovementKeys[x]))
                .ToArray();

            Move(Tools.MovementKeys[Client.ReadInput(validKeys)]);

            Client.SendVisibleLevelState(1);
        }
    }
}
