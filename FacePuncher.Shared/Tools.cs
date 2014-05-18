/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 * Copyright (C) 2014 Tamme Schichler (tammeschichler@googlemail.com)
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
using System.IO;
using System.Linq;
using System.Xml.Linq;

using FacePuncher.Geometry;
using FacePuncher.Network;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FacePuncher
{
    public static class Tools
    {
        public static readonly Random Random = new Random();

        public static readonly Direction[] Directions = new[] {
            Direction.NorthWest, Direction.North, Direction.NorthEast,
            Direction.West, Direction.East,
            Direction.SouthWest, Direction.South, Direction.SouthEast
        };

        public static Position GetOffset(this Direction dir)
        {
            return new Position(((int) dir) % 3 - 1, ((int) dir) / 3 - 1);
        }

        public static bool HasElement(this XElement elem, XName name)
        {
            return elem.Elements(name).Count() > 0;
        }

        public static T Element<T>(this XElement elem, XName name)
            where T : struct
        {
            return (T) elem.Element(name, typeof(T));
        }

        public static Object Element(this XElement elem, XName name, Type type)
        {
            if (type.IsEnum) {
                return Enum.Parse(type, elem.Element(name).Value, true);
            } else {
                return Convert.ChangeType(elem.Element(name).Value, type);
            }
        }

        public static int Clamp(this int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }

        public static void Write(this NetworkStream stream, Position pos)
        {
            stream.Write(pos.X);
            stream.Write(pos.Y);
        }

        public static async Task<Position> ReadPosition(this NetworkStream stream)
        {
            return new Position(
                x: await stream.ReadInt32(),
                y: await stream.ReadInt32());
        }

        public static void Write(this NetworkStream stream, Rectangle rect)
        {
            stream.Write(rect.Left);
            stream.Write(rect.Top);
            stream.Write(rect.Width);
            stream.Write(rect.Height);
        }

        public static async Task<Rectangle> ReadRectangle(this NetworkStream stream)
        {
            return new Rectangle(
                x: await stream.ReadInt32(),
                y: await stream.ReadInt32(),
                w: await stream.ReadInt32(),
                h: await stream.ReadInt32());
        }

        public static void WriteAppearance(this Stream stream, char symbol, ConsoleColor foreColor, ConsoleColor backColor)
        {
            stream.WriteByte((byte)(symbol >> 8));
            stream.WriteByte(unchecked((byte)symbol));
            stream.WriteByte((byte)((byte)foreColor | ((byte)backColor << 4)));
        }

        // TODO: split this method
        public static async Task<Tuple<char, ConsoleColor, ConsoleColor>> ReadAppearance(this NetworkStream stream)
        {
            var symbol = (char)(await stream.ReadByteAsync() << 8 | await stream.ReadByteAsync());

            int color = await stream.ReadByteAsync();

            var foreColor = (ConsoleColor)(color & 0xf);
            var backColor = (ConsoleColor)(color >> 4);

            return Tuple.Create(symbol, foreColor, backColor);
        }
    }
}
