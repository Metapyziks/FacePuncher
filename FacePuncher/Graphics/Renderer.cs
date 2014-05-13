using FacePuncher.Geometry;

namespace FacePuncher.Graphics
{
    static class Renderer
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
            for (int y = rect.Top; y < rect.Height; ++y) {
                for (int x = rect.Left; x < rect.Width; ++x) {
                    room[x, y].Draw(screenPos);
                }
            }
        }

        public static void Draw(this Tile tile, Position screenPos)
        {
            switch (tile.State) {
                case TileState.Void: return;
                case TileState.Wall: Display.SetCell(screenPos, '#'); break;
                case TileState.Floor: Display.SetCell(screenPos, '.'); break;
            }
        }
    }
}
