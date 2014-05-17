using FacePuncher.Geometry;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Abstract component for entities that should move
    /// of their own volition.
    /// </summary>
    abstract class AgentControl : Component
    {
        private ulong _nextMove;

        /// <summary>
        /// Gets or sets the number of game ticks between
        /// movements for this entity.
        /// </summary>
        [ScriptDefinable]
        public ulong MovePeriod { get; set; }

        /// <summary>
        /// Checks to see if the entity can move at the
        /// current time.
        /// </summary>
        protected bool CanMove
        {
            get { return Time >= _nextMove; }
        }

        /// <summary>
        /// Attempts to move the entity in the specified direction.
        /// </summary>
        /// <param name="dir">Direction to move.</param>
        /// <returns>Whether the move was successful.</returns>
        protected bool Move(Direction dir)
        {
            if (MovePeriod == 0 || !CanMove || !Entity.CanMove(dir)) return false;

            Entity.Move(dir);
            _nextMove = Time + MovePeriod;
            return true;
        }
    }
}
