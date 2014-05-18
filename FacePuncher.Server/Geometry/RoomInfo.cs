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

namespace FacePuncher.Geometry
{

    class RoomPlan
    {
        public int Index { get; private set; }

        public Rectangle Rect { get; private set; }

        public RoomGenerator Generator { get; private set; }

        public Dictionary<RoomPlan, Rectangle> Doors { get; private set; }

        public RoomPlan(Rectangle rect, RoomGenerator generator, int index)
        {
            Index = index;
            Rect = rect;
            Generator = generator;
            Doors = new Dictionary<RoomPlan, Rectangle>();
        }

        public void AddDoor(RoomPlan room, Rectangle rect)
        {
            Doors.Add(room, rect.Intersection(Rect) - Rect.TopLeft);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Rect.ToString(), Generator.Name);
        }
    }
}
