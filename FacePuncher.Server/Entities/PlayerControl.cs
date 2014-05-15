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
        private Intent _intent;

        public Intent Intent
        {
            get { return _intent; }
            set { _intent = value; }
        }

        public override void OnThink(ulong time)
        {
            Intent.HandleIntent(ref _intent, (MoveIntent mi) => HandleMove(mi, time)); // This isn't quite optimal

            Client.SendVisibleLevelState(Level, time);
        }

        private bool HandleMove(MoveIntent intent, ulong time)
        {
            var success = Move(intent.Direction, time);

            if (success)
            {
                // Let the client know what they can see in their new position.
                Client.SendVisibleLevelState(Level, time + 1);
            }

            return success;
        }
    }
}
