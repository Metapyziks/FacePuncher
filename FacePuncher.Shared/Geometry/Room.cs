using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// A rectangular region of tiles in a level.
    /// </summary>
    public class Room : IEnumerable<Tile>
    {
        /// <summary>
        /// Gets the level containing this room.
        /// </summary>
        public Level Level { get; private set; }

        /// <summary>
        /// Gets a rectangle representing the location and
        /// dimensions of this room within the level.
        /// </summary>
        public Rectangle Rect { get; private set; }

        /// <summary>
        /// Gets a rectangle with the width and height of this
        /// room, with the top left corner at the origin.
        /// </summary>
        public Rectangle RelativeRect { get { return new Rectangle(0, 0, Width, Height); } }

        /// <summary>
        /// Gets the horizontal position of the left edge of this room.
        /// </summary>
        public int Left { get { return Rect.Left; } }

        /// <summary>
        /// Gets the vertical position of the top edge of this room.
        /// </summary>
        public int Top { get { return Rect.Top; } }

        /// <summary>
        /// Gets the horizontal position of the right edge of this room.
        /// </summary>
        public int Right { get { return Rect.Right; } }

        /// <summary>
        /// Gets the vertical position of the bottom edge of this room.
        /// </summary>
        public int Bottom { get { return Rect.Bottom; } }

        /// <summary>
        /// Gets the horizontal size of this room.
        /// </summary>
        public int Width { get { return Rect.Width; } }

        /// <summary>
        /// Gets the vertical size of this room.
        /// </summary>
        public int Height { get { return Rect.Height; } }
        
        private Tile[,] _tiles;

        /// <summary>
        /// Constructs a new room instance within the given
        /// level with the specified bounding rectangle.
        /// </summary>
        /// <param name="level">Level to contain this room.</param>
        /// <param name="rect">Bounding rectangle for this room.</param>
        internal Room(Level level, Rectangle rect)
        {
            Level = level;
            Rect = rect;

            _tiles = new Tile[Width, Height];

            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    _tiles[x, y] = new Tile(this, x, y);
                }
            }
        }

        /// <summary>
        /// Gets an individual tile from the level from a position
        /// relative to the top left corner of this room.
        /// </summary>
        /// <param name="relX">Horizontal position of the tile relative
        /// to the left edge of this room.</param>
        /// <param name="relY">Vertical position of the tile relative
        /// to the top edge of this room.</param>
        /// <returns>A tile from the specified relative position.</returns>
        public Tile this[int relX, int relY]
        {
            get { return this[new Position(relX, relY)]; }
        }

        /// <summary>
        /// Gets an individual tile from the level from a position
        /// relative to the top left corner of this room.
        /// </summary>
        /// <param name="relPos">Position of the tile relative to the
        /// top left corner of this room.</param>
        /// <returns>A tile from the specified relative position.</returns>
        public Tile this[Position relPos]
        {
            get
            {
                if (relPos.X < 0 || relPos.Y < 0 || relPos.X >= Width || relPos.Y >= Height) {
                    return Level[Rect.TopLeft + relPos];
                }

                return _tiles[relPos.X, relPos.Y];
            }
        }

        /// <summary>
        /// Removes all walls and floors from the specified area.
        /// </summary>
        /// <param name="rect">Region to remove walls from.</param>
        public void ClearGeometry(Rectangle rect)
        {
            foreach (var pos in rect.Intersection(RelativeRect).Positions) {
                this[pos].State = TileState.Void;
            }
        }

        /// <summary>
        /// Sets each tile within the specified rectangle to be a wall.
        /// </summary>
        /// <param name="rect">Region to add walls to.</param>
        public void AddWalls(Rectangle rect)
        {
            foreach (var pos in rect.Intersection(RelativeRect).Positions) {
                this[pos].State = TileState.Wall;
            }
        }

        /// <summary>
        /// Sets each tile within the specified rectangle to be a floor.
        /// </summary>
        /// <param name="rect">Region to add walls to.</param>
        public void AddFloors(Rectangle rect)
        {
            foreach (var pos in rect.Intersection(RelativeRect).Positions) {
                this[pos].State = TileState.Floor;
            }
        }

        /// <summary>
        /// Instruct each entity within the room to perform a single think step.
        /// </summary>
        public void Think()
        {
            foreach (var tile in _tiles) {
                tile.Think();
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through each tile in the room.
        /// </summary>
        /// <returns>An enumerator that iterates through each tile in the room.</returns>
        public IEnumerator<Tile> GetEnumerator()
        {
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    yield return _tiles[x, y];
                }
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through each tile in the room.
        /// </summary>
        /// <returns>An enumerator that iterates through each tile in the room.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
