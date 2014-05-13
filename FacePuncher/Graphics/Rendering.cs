using System;
using System.Linq;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher.Graphics
{
    public struct DrawAttributes
    {
        public Position Origin;

        public int Flash;

        public DrawAttributes(Position origin, int flash)
        {
            Origin = origin;
            Flash = flash;
        }
    }

    static class Rendering
    {
        public const int EntityFlashPeriod = 8;

        static readonly char[] _sWallTiles = new[] {
            '\u006f', '\u006f', '\u006f', '\u00c9',
            '\u006f', '\u00cd', '\u00bb', '\u00cb',
            '\u006f', '\u00c8', '\u00ba', '\u00cc',
            '\u00bc', '\u00ca', '\u00b9', '\u00ce'
        };

        public static void Draw(this Level level, Rectangle rect, Position screenPos, DrawAttributes attribs)
        {
            foreach (var room in level.GetIntersectingRooms(rect)) {
                var subRect = room.Rect.Intersection(rect);
                var roomPos = room.Rect.TopLeft;

                room.Draw(subRect - roomPos, screenPos + roomPos - rect.TopLeft, attribs);
            }
        }

        public static void Draw(this Room room, Rectangle rect, Position screenPos, DrawAttributes attribs)
        {
            foreach (var pos in rect.Positions) {
                room[pos].Draw(screenPos + pos, attribs);
            }
        }

        public static void Draw(this Tile tile, Position screenPos, DrawAttributes attribs)
        {
            if (tile.State == TileState.Void) return;

            if (!tile.IsVisibleFrom(attribs.Origin, 12)) return;

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

                var drawable = drawables[EntityFlashPeriod % drawables.Length];

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
