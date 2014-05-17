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

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Enumeration of each of the eight
    /// directions an entity may move to
    /// a neighbouring tile.
    /// </summary>
    /// <remarks>
    /// Values are organised like this:
    /// 
    /// 0 | 1 | 2
    /// 3 | 4 | 5
    /// 6 | 7 | 8
    /// </remarks>
    public enum Direction
    {
        NorthWest = 0,
        North = 1,
        NorthEast = 2,
        West = 3,
        None = 4,
        East = 5,
        SouthWest = 6,
        South = 7,
        SouthEast = 8
    }
}
