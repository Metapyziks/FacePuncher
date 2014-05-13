using System;

using FacePuncher.Geometry;

namespace FacePuncher.Graphics
{
    static class Rendering
    {
        public static void Draw(this Level level, Rectangle rect, Position screenPos)
        {
            foreach (var room in level.GetIntersectingRooms(rect)) {
                var subRect = room.Rect.Intersection(rect);
                var roomPos = room.Rect.TopLeft;
                
                room.Draw(subRect - roomPos, screenPos + roomPos - rect.TopLeft);
            }
        }

        public static void Draw(this Room room, Rectangle rect, Position screenPos)
        {
            foreach (var pos in rect.Positions) {
                room[pos].Draw(screenPos + pos);
            }
        }

        public static void Draw(this Tile tile, Position screenPos)
        {
            switch (tile.State) {
                case TileState.Void: return;
                case TileState.Wall: Display.SetCell(screenPos, '#'); break;
                case TileState.Floor: Display.SetCell(screenPos, '.', ConsoleColor.DarkGray); break;
            }
        }
    }
}
