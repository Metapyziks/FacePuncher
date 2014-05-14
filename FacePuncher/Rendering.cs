using System;
using System.Collections.Generic;
using System.Linq;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    /// <summary>
    /// Object containing information used when drawing 
    /// tiles and entities.
    /// </summary>
    public class DrawAttributes
    {
        /// <summary>
        /// Current game time at the time of drawing.
        /// </summary>
        public ulong Time { get; private set; }

        /// <summary>
        /// Flash state for animating entities.
        /// </summary>
        public int Flash { get; private set; }

        /// <summary>
        /// Initializes a DrawAttributes structure.
        /// </summary>
        /// <param name="time">Current game time at the time of drawing.</param>
        /// <param name="flash">Flash state for animating entities.</param>
        public DrawAttributes(ulong time, int flash)
        {
            Time = time;
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
        static readonly char[] _sWallTiles = new[] {
            '\u006f', '\u006f', '\u006f', '\u00c9',
            '\u006f', '\u00cd', '\u00bb', '\u00cb',
            '\u006f', '\u00c8', '\u00ba', '\u00cc',
            '\u00bc', '\u00ca', '\u00b9', '\u00ce'
        };

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
        public static void Draw(this RoomVisibility vis, Rectangle rect, Position screenPos, DrawAttributes attribs)
        {
            var subRect = vis.Room.Rect.Intersection(rect);
            var roomPos = vis.Room.Rect.TopLeft;

            // Move the screen position and clipping rectangle to room-space.
            screenPos += roomPos - rect.TopLeft;
            rect = subRect - roomPos;

            // Draw each visible tile that is within the clipping rectangle.
            foreach (var tile in vis.GetVisible(attribs.Time)) {
                if (rect.Intersects(tile.RelativePosition)) {
                    tile.Draw(screenPos + tile.RelativePosition, attribs, true);
                }
            }

            // Draw each hidden but rememberd tile that is within the clipping rectangle.
            foreach (var tile in vis.GetRemembered(attribs.Time)) {
                if (rect.Intersects(tile.RelativePosition)) {
                    tile.Draw(screenPos + tile.RelativePosition, new DrawAttributes(attribs.Time, 0), false);
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
        public static void Draw(this Tile tile, Position screenPos, DrawAttributes attribs, bool visible)
        {
            // Don't draw nothing.
            if (tile.State == TileState.Void) return;

            // If the tile is a wall, find which character to use.
            if (tile.State == TileState.Wall) {
                int adj = 
                    (tile.GetNeighbour(Direction.East).State  == TileState.Wall ? 1 : 0) |
                    (tile.GetNeighbour(Direction.South).State == TileState.Wall ? 2 : 0) |
                    (tile.GetNeighbour(Direction.West).State  == TileState.Wall ? 4 : 0) |
                    (tile.GetNeighbour(Direction.North).State == TileState.Wall ? 8 : 0);

                Display.SetCell(screenPos, _sWallTiles[adj], visible ? ConsoleColor.Blue : ConsoleColor.DarkGray);
                return;
            }

            // Get an array of all entities that may be drawn.
            var drawables = tile.Entities
                .Where(x => x.HasComponent<Drawable>())
                .Select(x => x.GetComponent<Drawable>())
                .ToArray();
            
            // If there's something to draw...
            if (drawables.Length > 0) {
                // Find which layer is the upper-most.
                var layer = (DrawableLayer) drawables.Max(x => x.Layer);

                // Find all drawables that are on that layer.
                drawables = drawables
                    .Where(x => x.Layer == layer)
                    .ToArray();

                // Select the drawable to draw based on the flash state.
                int index = (attribs.Flash / EntityFlashPeriod) % drawables.Length;
                var drawable = drawables[index];

                // Draw the entity.
                Display.SetCell(screenPos,
                    drawable.GetSymbol(attribs),
                    visible ? drawable.GetForeColor(attribs) : ConsoleColor.DarkGray,
                    visible ? drawable.GetBackColor(attribs) : ConsoleColor.Black);

                return;
            }

            // When no drawable entities are present just draw the floor.
            // TODO: Allow multiple types of floor with different symbols,
            //       definable in definition files.
            Display.SetCell(screenPos, '+', visible ? ConsoleColor.DarkBlue : ConsoleColor.DarkGray);
        }
    }
}
