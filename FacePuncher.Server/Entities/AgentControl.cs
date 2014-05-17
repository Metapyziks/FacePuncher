/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Tamme Schichler (tammeschichler@googlemail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

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
