/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
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
        /// <summary>
        /// Gets or sets the number of game ticks between
        /// movements for this entity.
        /// </summary>
        [ScriptDefinable]
        public double MovePeriod { get; set; }
        
        /// <summary>
        /// Attempts to move the entity in the specified direction.
        /// </summary>
        /// <param name="dir">Direction to move.</param>
        /// <returns>Whether the move was successful.</returns>
        protected Delay Move(Direction dir)
        {
            if (!Entity.CanMove(dir)) return Delay(0);

            Entity.Move(dir);

            return Delay(MovePeriod);
        }
    }
}
