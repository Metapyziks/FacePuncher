/* Copyright (c) 2014 James King [metapyziks@gmail.com]
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

using System;
using System.Collections.Generic;
using System.Linq;

using FacePuncher.Graphics;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Records the visibility of tiles within a room.
    /// </summary>
    public class RoomVisibility
    {
        private ulong[,] _mask;
        private TileAppearance[,] _tiles;

        /// <summary>
        /// Region of the level this visibility instance is representing.
        /// </summary>
        public Rectangle Rect { get; private set; }

        /// <summary>
        /// Gets the latest time any tile within the room was seen.
        /// </summary>
        public ulong LastVisibleTime { get; private set; }

        /// <summary>
        /// Creates a new RoomVisibility instance to track the visibility
        /// of tiles within a specified room.
        /// </summary>
        /// <param name="room">Room to track the visibility of.</param>
        public RoomVisibility(Rectangle rect)
        {
            Rect = rect;
            LastVisibleTime = 0;

            _mask = new ulong[Rect.Width, Rect.Height];
            _tiles = new TileAppearance[Rect.Width, Rect.Height];
        }
        
        /// <summary>
        /// Marks a specific tile within the room as being visible
        /// at the given time.
        /// </summary>
        /// <param name="pos">Position of the tile to mark as visible.</param>
        /// <param name="appearance">Appearance of the tile at the time
        /// of being seen.</param>
        /// <param name="time">Time at which the tile was seen.</param>
        public void Reveal(Position pos, TileAppearance appearance, ulong time)
        {
            _mask[pos.X, pos.Y] = Math.Max(_mask[pos.X, pos.Y], time);
            _tiles[pos.X, pos.Y] = appearance;

            LastVisibleTime = Math.Max(LastVisibleTime, time);
        }
        
        /// <summary>
        /// Gets a set of all tiles within the room that were visible at the
        /// specified time.
        /// </summary>
        /// <param name="time">Time at which to compare visibility.</param>
        /// <returns>A set of visible tiles.</returns>
        public IEnumerable<TileAppearance> GetVisible(ulong time)
        {
            if (LastVisibleTime < time) return Enumerable.Empty<TileAppearance>();

            return (Rect - Rect.TopLeft).Positions
                .Where(p => _mask[p.X, p.Y] >= time)
                .Select(p => _tiles[p.X, p.Y]);
        }

        /// <summary>
        /// Gets a set of all tiles within the room that were not visible at
        /// the specified time but have been seen in the past.
        /// </summary>
        /// <param name="time">Time at which to compare visibility.</param>
        /// <returns>A set of previously seen tiles.</returns>
        public IEnumerable<TileAppearance> GetRemembered(ulong time)
        {
            if (LastVisibleTime == 0) return Enumerable.Empty<TileAppearance>();

            return (Rect - Rect.TopLeft).Positions
                .Where(p => _mask[p.X, p.Y] != 0 && _mask[p.X, p.Y] < time)
                .Select(p => _tiles[p.X, p.Y]);
        }
    }
}
