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
            // Check to see if no client is attached or the entity can't move.
            if (Client == null || !CanMove(time)) return;

            // Make sure the client is up-to-date with the world before
            // they choose to act.
            Client.SendVisibleLevelState(Level, time);

            // Find the movement keys corresponding to the directions this
            // entity can move.
            var validKeys = Tools.MovementKeys.Keys
                .Where(x => Entity.CanMove(Tools.MovementKeys[x]))
                .ToArray();

            // Move in the direction the client specifies.
            Move(Tools.MovementKeys[Client.ReadInput(validKeys)], time);

            // Let the client know what they can see in their new position.
            Client.SendVisibleLevelState(Level, time + 1);
        }
    }
}
