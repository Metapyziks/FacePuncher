using System.Collections.Generic;
using System.Linq;

namespace FacePuncher.Geometry
{
    public class RoomVisibility
    {
        private ulong[,] _mask;

        public Room Room { get; private set; }

        public RoomVisibility(Room room)
        {
            Room = room;

            _mask = new ulong[room.Width, room.Height];
        }
        
        public bool UpdateVisibility(Position origin, int maxRadius, ulong time)
        {
            if ((Room.Rect.NearestPosition(origin) - origin).LengthSquared > maxRadius * maxRadius) {
                return false;
            }

            bool visible = false;

            foreach (var pos in Room.Rect.Positions) {
                if ((pos - origin).LengthSquared > maxRadius * maxRadius) continue;

                var rel = pos - Room.Rect.TopLeft;

                if (_mask[rel.X, rel.Y] >= time) continue;

                foreach (var mid in origin.BresenhamLine(pos)) {
                    rel = mid - Room.Rect.TopLeft;

                    if (Room.RelativeRect.Intersects(rel)) {
                        visible = true;
                        _mask[rel.X, rel.Y] = time;
                    }

                    if (Room[rel].State != TileState.Floor) break;
                }
            }

            return visible;
        }

        public IEnumerable<Tile> GetVisible(ulong time)
        {
            return Room.RelativeRect.Positions
                .Where(p => _mask[p.X, p.Y] >= time)
                .Select(p => Room[p]);
        }
    }
}
