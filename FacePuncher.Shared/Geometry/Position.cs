/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    /// <summary>
    /// Structure representing a location in Cartesian coordinates.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// A position representing the origin.
        /// </summary>
        public static readonly Position Zero = new Position(0, 0);

        /// <summary>
        /// A position representing one unit in the horizontal axis.
        /// </summary>
        public static readonly Position UnitX = new Position(1, 0);

        /// <summary>
        /// A position representing one unit in the vertical axis.
        /// </summary>
        public static readonly Position UnitY = new Position(0, 1);

        /// <summary>
        /// Horizontal component of the position.
        /// </summary>
        public int X;

        /// <summary>
        /// Vertical component of the position.
        /// </summary>
        public int Y;

        /// <summary>
        /// Gets the squared euclidean distance from the origin to
        /// this position.
        /// </summary>
        public int LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        /// <summary>
        /// Gets the manhattan distance from the origin to this position.
        /// </summary>
        public int ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y); }
        }

        /// <summary>
        /// Initializes a new position structure with the given coordinates.
        /// </summary>
        /// <param name="x">Horizontal component of the position.</param>
        /// <param name="y">Vertical component of the position.</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets a sequence of positions that trace a line from
        /// this position to the specified destination.
        /// </summary>
        /// <param name="dest">Destination to trace a line to.</param>
        /// <returns>A sequence of positions forming a line.</returns>
        /// <remarks>
        /// From http://bit.ly/QNHZ7i
        /// </remarks>
        public IEnumerable<Position> BresenhamLine(Position dest)
        {
            var a = this;
            var b = dest;

            int dx = Math.Abs(b.X - a.X);
            int dy = Math.Abs(b.Y - a.Y);

            int sx = a.X < b.X ? 1 : -1;
            int sy = a.Y < b.Y ? 1 : -1;
            int err = dx - dy;

            for (;;) {
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

        #region Operator Overloads
        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator +(Position P, int[] Sz)
        {
            return new Position(P.X + Sz[0], P.Y + Sz[1]);
        }

        public static Position operator +(Position P, int I)
        {
            return P + new[] { I, 0 };
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public static Position operator -(Position P, int[] Sz)
        {
            return new Position(P.X - Sz[0], P.Y - Sz[1]);
        }

        public static Position operator -(Position P, int I)
        {
            return P - new[] { I, 0 };
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
        #endregion

        /// <summary>
        /// Indicates whether this instance and another specified
        /// object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if this instance is equal to the given object,
        /// and false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Position && this == ((Position) obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return X ^ Y;
        }

        /// <summary>
        /// Returns a string representing this position.
        /// </summary>
        /// <returns>A string representing this position.</returns>
        public override string ToString()
        {
            return String.Format("({0} {1})", X, Y);
        }
    }
}
