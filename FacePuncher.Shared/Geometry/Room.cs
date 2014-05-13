using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    public class Room : IEnumerable<Tile>
    {
        public Level Level { get; private set; }

        public Rectangle Rect { get; private set; }

        public int Left { get { return Rect.Left; } }
        public int Top { get { return Rect.Top; } }
        public int Right { get { return Rect.Right; } }
        public int Bottom { get { return Rect.Bottom; } }

        public int Width { get { return Rect.Width; } }
        public int Height { get { return Rect.Height; } }
        
        private Tile[,] _tiles;

        public Room(Level level, Rectangle rect)
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

        public Tile this[int relX, int relY]
        {
            get
            {
                return this[new Position(relX, relY)];
            }
        }

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

        public void ClearGeometry(Rectangle rect)
        {
            foreach (var pos in rect.Positions) {
                this[pos].State = TileState.Void;
            }
        }

        public void AddGeometry(Rectangle rect)
        {
            foreach (var pos in rect.Positions) {
                this[pos].State = TileState.Wall;
            }
        }

        public void SubtractGeometry(Rectangle rect)
        {
            foreach (var pos in rect.Positions) {
                this[pos].State = TileState.Floor;
            }
        }

        public void Think(ulong time)
        {
            foreach (var tile in _tiles) {
                tile.Think(time);
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    yield return _tiles[x, y];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
