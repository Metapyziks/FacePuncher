/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace FacePuncher.Geometry
{
    static partial class Visibility
    {
        struct Vector2
        {
            public static readonly Vector2 Zero = new Vector2(0, 0);
            public static readonly Vector2 UnitX = new Vector2(1, 0);
            public static readonly Vector2 UnitY = new Vector2(0, 1);

            public float X;
            public float Y;

            public float Length
            {
                get { return (float) Math.Sqrt(X * X + Y * Y); }
            }

            public float LengthSquared
            {
                get { return X * X + Y * Y; }
            }

            public Vector2(float x, float y)
            {
                X = x; Y = y;
            }

            public Vector2 Normalized()
            {
                var len = Length;
                if (len == 0) return Zero;
                return this / LengthSquared;
            }

            public float Dot(Vector2 that)
            {
                return this.X * that.X + this.Y * that.Y;
            }

            public static Vector2 operator -(Vector2 vec)
            {
                return new Vector2(-vec.X, -vec.Y);
            }

            public static Vector2 operator +(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2 operator -(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X - b.X, a.Y - b.Y);
            }

            public static Vector2 operator +(Position a, Vector2 b)
            {
                return new Vector2(a.X + b.X, a.Y + b.Y);
            }

            public static Vector2 operator -(Position a, Vector2 b)
            {
                return new Vector2(a.X - b.X, a.Y - b.Y);
            }

            public static Vector2 operator *(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X * b.X, a.Y * b.Y);
            }

            public static Vector2 operator *(Vector2 vec, float mul)
            {
                return new Vector2(vec.X * mul, vec.Y * mul);
            }

            public static Vector2 operator /(Vector2 a, Vector2 b)
            {
                return new Vector2(a.X / b.X, a.Y / b.Y);
            }

            public static Vector2 operator /(Vector2 vec, float div)
            {
                return new Vector2(vec.X / div, vec.Y / div);
            }

            public override string ToString()
            {
                return String.Format("({0} {1})", X, Y);
            }
        }
    }
}
