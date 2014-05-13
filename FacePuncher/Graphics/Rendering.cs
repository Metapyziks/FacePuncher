using System;
using System.Linq;

using FacePuncher.Entities;
using FacePuncher.Geometry;

namespace FacePuncher.Graphics
{
    public struct DrawAttributes
    {
        public int Flash;

        public DrawAttributes(int flash)
        {
            Flash = flash;
        }
    }

    static class Rendering
    {
        public const int EntityFlashPeriod = 8;

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

            if (tile.State == TileState.Wall) {
                Display.SetCell(screenPos, '#');
                return;
            }

            var drawables = tile.Entities
                .Where(x => x.HasComponent<Drawable>())
                .Select(x => x.GetComponent<Drawable>())
                .ToArray();

            if (drawables.Length > 0) {
                var drawable = drawables[EntityFlashPeriod % drawables.Length];

                Display.SetCell(screenPos,
                    drawable.GetSymbol(attribs),
                    drawable.GetForeColor(attribs),
                    drawable.GetBackColor(attribs));

                return;
            }

            Display.SetCell(screenPos, '.', ConsoleColor.DarkGray);
        }
    }
}
