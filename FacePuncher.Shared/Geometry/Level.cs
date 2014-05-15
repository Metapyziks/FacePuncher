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

        public ulong Time { get; set; }

        /// <summary>
        /// Constructs an empty level.
        /// </summary>
        public Level()
        {
            _rooms = new List<Room>();

            Time = 1;
        }

        /// <summary>
        /// Creates a new room with the specified rectangle and
        /// adds it to the level.
        /// </summary>
        /// <param name="rect">Rectangle that specifies the location
        /// and dimensions of the new room.</param>
        /// <returns>A newly created room.</returns>
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

        public void Think()
        {
            foreach (var room in _rooms) {
                room.Think();
            }

            ++Time;
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
