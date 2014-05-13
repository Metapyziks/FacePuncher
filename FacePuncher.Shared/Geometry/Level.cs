using System.Collections.Generic;
using System.Linq;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// A class representing a game level as a collection of rooms.
    /// </summary>
    public class Level : IEnumerable<Room>
    {
        private List<Room> _rooms;

        /// <summary>
        /// Constructs an empty level.
        /// </summary>
        public Level()
        {
            _rooms = new List<Room>();
        }

        public Room CreateRoom(Rectangle rect)
        {
            var room = new Room(this, rect);

            _rooms.Add(room);

            return room;
        }

        public IEnumerable<Room> GetIntersectingRooms(Rectangle rect)
        {
            return _rooms.Where(x => x.Rect.Intersects(rect));
        }

        public Tile this[int x, int y]
        {
            get
            {
                return this[new Position(x, y)];
            }
        }

        public Tile this[Position pos]
        {
            get
            {
                var room = _rooms.FirstOrDefault(x => x.Rect.Intersects(pos));
                if (room == null) return Tile.Default;

                return room[pos - room.Rect.TopLeft];
            }
        }

        public void Think(ulong time)
        {
            foreach (var room in _rooms) {
                room.Think(time);
            }
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }
    }
}
