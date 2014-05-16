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
        /// Gets or sets the current game time in steps.
        /// </summary>
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

        /// <summary>
        /// Gets the set of rooms that intersect a given rectangle.
        /// </summary>
        /// <param name="rect">Rectangle to test for intersections.</param>
        /// <returns>A set of rooms that intersect with the given rectangle.</returns>
        public IEnumerable<Room> GetIntersectingRooms(Rectangle rect)
        {
            return _rooms.Where(x => x.Rect.Intersects(rect));
        }

        /// <summary>
        /// Gets a tile at the specified location in the level.
        /// </summary>
        /// <param name="x">Horizontal position of the tile.</param>
        /// <param name="y">Vertical position of the tile.</param>
        /// <returns>A tile from the specified location.</returns>
        public Tile this[int x, int y]
        {
            get { return this[new Position(x, y)]; }
        }

        /// <summary>
        /// Gets a tile at the specified location in the level.
        /// </summary>
        /// <param name="pos">Position of the tile to get.</param>
        /// <returns>A tile from the specified location.</returns>
        public Tile this[Position pos]
        {
            get
            {
                var room = _rooms.FirstOrDefault(x => x.Rect.Intersects(pos));
                if (room == null) return Tile.Default;

                return room[pos - room.Rect.TopLeft];
            }
        }

        /// <summary>
        /// Perform a single think step and increment the time counter.
        /// </summary>
        public void Think()
        {
            foreach (var room in _rooms) room.Think();

            ++Time;
        }

        /// <summary>
        /// Gets an enumerator that iterates through each room in the level.
        /// </summary>
        /// <returns>An enumerator that iterates through each room in the level.</returns>
        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through each room in the level.
        /// </summary>
        /// <returns>An enumerator that iterates through each room in the level.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }
    }
}
