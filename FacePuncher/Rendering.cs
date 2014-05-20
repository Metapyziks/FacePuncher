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
using System.Linq;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    /// <summary>
    /// Structure containing information used when drawing 
    /// tiles and entities.
    /// </summary>
    public struct DrawAttributes
    {
        /// <summary>
        /// Flash state for animating entities.
        /// </summary>
        public readonly int Flash;

        /// <summary>
        /// Initializes a DrawAttributes structure.
        /// </summary>
        /// <param name="flash">Flash state for animating entities.</param>
        public DrawAttributes(int flash)
        {
            Flash = flash;
        }
    }

    /// <summary>
    /// Utility class for drawing tiles and entities.
    /// </summary>
    static class Rendering
    {
        /// <summary>
        /// The period between cycling between different entities
        /// when multiple entities are present within the same layer.
        /// </summary>
        public const int EntityFlashPeriod = 6;

        /// <summary>
        /// A map of the 16 possible neighbouring tile solidity
        /// states to the characters that should be used when
        /// drawing walls.
        /// 
        /// TODO: Move to a definition file, allowing for different
        ///       types of walls?
        /// </summary>

        /// <summary>
        /// Renders the tiles deemed visible within the room corresponding
        /// to this visibility mask to the specified location on the screen.
        /// </summary>
        /// <param name="vis">Visibility mask to draw from.</param>
        /// <param name="rect">Clipping rectangle in level-space.</param>
        /// <param name="screenPos">Position on the screen that the top-left
        /// of the rectangle should be drawn to.</param>
        /// <param name="attribs">Attributes to be used when drawing tiles
        /// and entities.</param>
        public static void Draw(this RoomVisibility vis, Rectangle rect, Position screenPos, DrawAttributes attribs, ulong time)
        {
            var subRect = vis.Rect.Intersection(rect);
            var roomPos = vis.Rect.TopLeft;

            if (subRect.Width * subRect.Height <= 0) return;

            // Move the screen position and clipping rectangle to room-space.
            screenPos += roomPos - rect.TopLeft;
            rect = subRect - roomPos;

            foreach (var tile in vis.GetVisible(time)) {
                if (rect.Intersects(tile.Position)) {
                    tile.Draw(screenPos + tile.Position, attribs, true);
                }
            }

            foreach (var tile in vis.GetRemembered(time)) {
                if (rect.Intersects(tile.Position)) {
                    tile.Draw(screenPos + tile.Position, new DrawAttributes(0), false);
                }
            }
        }

        /// <summary>
        /// Renders an individual tile and any entities it contains to the
        /// specified location on the screen.
        /// </summary>
        /// <param name="tile">Tile to draw.</param>
        /// <param name="screenPos">Position on the screen to draw the tile.</param>
        /// <param name="attribs">Attributes to be used when drawing the tile and any entities.</param>
        /// <param name="visible">If true, the tile is currently within visible range.</param>
        public static void Draw(this TileAppearance tile, Position screenPos, DrawAttributes attribs, bool visible)
        {
            if (tile.EntityCount == 0) {
                Interface.Display.SetCell(screenPos, tile.Symbol,
                    visible ? tile.ForeColor : ConsoleColor.DarkBlue,
                    visible ? tile.BackColor : ConsoleColor.Black);
            } else {
                int index = (attribs.Flash / EntityFlashPeriod) % tile.EntityCount;
                var entity = tile[index];
                var frame = entity[attribs.Flash % entity.FrameCount];

                Interface.Display.SetCell(screenPos, frame.Symbol,
                    visible ? frame.ForeColor : ConsoleColor.DarkBlue,
                    visible ? frame.BackColor : ConsoleColor.Black);
            }
        }
    }
}
