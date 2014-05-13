using System.Collections.Generic;

using FacePuncher.Entities;

namespace FacePuncher.Geometry
{
    class Tile : IEnumerable<Entity>
    {
        private List<Entity> _entities;

        public Room Room { get; private set; }

        /// <summary>
        /// Position of the tile relative to its containing room.
        /// </summary>
        public Position RelativePosition { get; private set; }

        public int RelativeX { get { return RelativePosition.X; } }
        public int RelativeY { get { return RelativePosition.Y; } }

        public Position Position { get { return Room.Rect.TopLeft + RelativePosition; } }

        public int X { get { return Room.Left + RelativeX; } }
        public int Y { get { return Room.Top + RelativeY; } }

        public Tile(Room room, Position relPos)
        {
            Room = room;
            RelativePosition = relPos;

            _entities = new List<Entity>();
        }

        public Tile(Room room, int relX, int relY)
            : this(room, new Position(relX, relY)) { }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }
    }
}
