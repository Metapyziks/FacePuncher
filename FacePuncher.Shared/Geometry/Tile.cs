using System.Collections.Generic;

using FacePuncher.Entities;

namespace FacePuncher.Geometry
{
    public enum TileState
    {
        Void = 0,
        Wall = 1,
        Floor = 2
    }

    /// <summary>
    /// Class representing an individual tile in a room.
    /// </summary>
    public class Tile : IEnumerable<Entity>
    {
        public static readonly Tile Default = new Tile(null, 0, 0);

        private List<Entity> _entities;

        /// <summary>
        /// Parent room containing this tile.
        /// </summary>
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

        public TileState State { get; set; }

        public IEnumerable<Entity> Entities { get { return _entities; } }
        
        public Tile(Room room, Position relPos)
        {
            Room = room;
            RelativePosition = relPos;

            State = TileState.Void;

            _entities = new List<Entity>();
        }

        public Tile(Room room, int relX, int relY)
            : this(room, new Position(relX, relY)) { }

        internal void AddEntity(Entity ent)
        {
            if (State == TileState.Void) return;

            if (ent.Tile != this) return;
            if (_entities.Contains(ent)) return;

            _entities.Add(ent);
        }

        internal void RemoveEntity(Entity ent)
        {
            if (ent.Tile == this) return;
            if (!_entities.Contains(ent)) return;

            _entities.Remove(ent);
        }

        public Tile GetNeighbour(Position offset)
        {
            return Room[RelativePosition + offset];
        }

        public Tile GetNeighbour(Direction dir)
        {
            return GetNeighbour(new Position(((int) dir) % 3 - 1, ((int) dir) / 3 - 1));
        }

        public void Think(ulong time)
        {
            for (int i = _entities.Count - 1; i >= 0; --i) {
                _entities[i].Think(time);
            }
        }

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
