using System;
using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Structure representing a location in Cartesian coordinates.
    /// </summary>
    public struct Position
    {
        public static readonly Position Zero = new Position(0, 0);
        public static readonly Position UnitX = new Position(1, 0);
        public static readonly Position UnitY = new Position(0, 1);

        /// <summary>
        /// Horizontal component of the position.
        /// </summary>
        public int X;

        /// <summary>
        /// Vertical component of the position.
        /// </summary>
        public int Y;

        public int LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        /// <summary>
        /// Constructs a position with the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public static Position operator *(Position pos, int mul)
        {
            return new Position(pos.X * mul, pos.Y * mul);
        }

        public static Position operator *(int mul, Position pos)
        {
            return new Position(pos.X * mul, pos.Y * mul);
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        
        public IEnumerable<Position> BresenhamLine(Position dest)
        {
            var a = this;
            var b = dest;

            int dx = Math.Abs(b.X - a.X);
            int dy = Math.Abs(b.Y - a.Y);

            int sx = a.X < b.X ? 1 : -1;
            int sy = a.Y < b.Y ? 1 : -1;
            int err = dx - dy;

            for (; ; ) {
                yield return a;
                if (a.X == b.X && a.Y == b.Y) yield break;

                int e2 = 2 * err;
                if (e2 > -dy) {
                    err = err - dy;
                    a.X += sx;
                }
                if (e2 < dx) {
                    err = err + dx;
                    a.Y += sy;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Position && this == ((Position) obj);
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return String.Format("({0} {1})", X, Y);
        }
    }
}
