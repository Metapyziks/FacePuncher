using System;
using System.Linq;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher
{
    /// <summary>
    /// Structure containing information used when
    /// drawing tiles and entities.
    /// </summary>
    public struct DrawAttributes
    {
        /// <summary>
        /// Current game time at the time of drawing.
        /// </summary>
        public ulong Time;

        /// <summary>
        /// Flash state for animating entities.
        /// </summary>
        public int Flash;

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
        /// 
        /// </summary>
        public const int EntityFlashPeriod = 8;

        static readonly char[] _sWallTiles = new[] {
            '\u006f', '\u006f', '\u006f', '\u00c9',
            '\u006f', '\u00cd', '\u00bb', '\u00cb',
            '\u006f', '\u00c8', '\u00ba', '\u00cc',
            '\u00bc', '\u00ca', '\u00b9', '\u00ce'
        };

        public static void Draw(this RoomVisibility vis, Rectangle rect, Position screenPos, DrawAttributes attribs)
        {
            var subRect = vis.Room.Rect.Intersection(rect);
            var roomPos = vis.Room.Rect.TopLeft;

            screenPos += roomPos - rect.TopLeft;
            rect = subRect - roomPos;

            foreach (var tile in vis.GetVisible(attribs.Time)) {
                if (rect.Intersects(tile.RelativePosition)) {
                    tile.Draw(screenPos + tile.RelativePosition, attribs);
                }
            }
        }

        public static void Draw(this Tile tile, Position screenPos, DrawAttributes attribs)
        {
            if (tile.State == TileState.Void) return;

            if (tile.State == TileState.Wall) {
                int adj = 
                    (tile.GetNeighbour(Direction.East).State  == TileState.Wall ? 1 : 0) |
                    (tile.GetNeighbour(Direction.South).State == TileState.Wall ? 2 : 0) |
                    (tile.GetNeighbour(Direction.West).State  == TileState.Wall ? 4 : 0) |
                    (tile.GetNeighbour(Direction.North).State == TileState.Wall ? 8 : 0);

                Display.SetCell(screenPos, _sWallTiles[adj]);
                return;
            }

            var drawables = tile.Entities
                .Where(x => x.HasComponent<Drawable>())
                .Select(x => x.GetComponent<Drawable>())
                .ToArray();
            
            if (drawables.Length > 0) {
                var layer = (DrawableLayer) drawables.Max(x => x.Layer);

                drawables = drawables
                    .Where(x => x.Layer == layer)
                    .ToArray();

                var drawable = drawables[(attribs.Flash / EntityFlashPeriod) % drawables.Length];

                Display.SetCell(screenPos,
                    drawable.GetSymbol(attribs),
                    drawable.GetForeColor(attribs),
                    drawable.GetBackColor(attribs));

                return;
            }

            Display.SetCell(screenPos, '+', ConsoleColor.DarkGray);
        }
    }
}
