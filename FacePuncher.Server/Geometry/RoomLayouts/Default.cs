/* Copyright (C) 2014 James King (metapyziks@gmail.com)
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

using System;
using System.Collections.Generic;

namespace FacePuncher.Geometry.RoomLayouts
{
    class Default : RoomLayout
    {
        [ScriptDefinable]
        public int MinLength { get; set; }

        [ScriptDefinable]
        public int MaxLength { get; set; }

        [ScriptDefinable]
        public int MinBreadth { get; set; }

        [ScriptDefinable]
        public int MaxBreadth { get; set; }

        public Default()
        {
            MinLength = 6;
            MaxLength = 6;
            MinBreadth = 6;
            MaxBreadth = 6;
        }

        public override void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            base.LoadFromDefinition(elem);
        }

        public override Position GenerateSize(Random rand)
        {
            if (rand.NextDouble() < 0.5) {
                return new Position(rand.Next(MinLength, MaxLength + 1), rand.Next(MinBreadth, MaxBreadth + 1));
            } else {
                return new Position(rand.Next(MinBreadth, MaxBreadth + 1), rand.Next(MinLength, MaxLength + 1));
            }
        }

        public override IEnumerable<Room> Generate(Level level, Rectangle rect, Rectangle[] doors, Random rand)
        {
            var room = level.CreateRoom(rect);
            rect -= rect.TopLeft;

            room.CreateWall(rect);
            room.CreateFloor(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

            foreach (var door in doors) room.CreateFloor(door);

            return new[] { room };
        }
    }
}
