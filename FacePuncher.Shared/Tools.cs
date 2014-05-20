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
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;

using FacePuncher.Geometry;
using FacePuncher.Network;

namespace FacePuncher
{
    public static class Tools
    {
        private static Stopwatch SWatch;

        static Tools()
        {
            SWatch = new Stopwatch();
            SWatch.Start();
        }

        public static readonly Random Random = new Random();

        public static readonly Direction[] Directions = new[] {
            Direction.East,
            Direction.South,
            Direction.West,
            Direction.North,
            Direction.SouthEast,
            Direction.SouthWest,
            Direction.NorthWest,
            Direction.NorthEast
        };

        public static readonly Direction[] CardinalDirections = new[] {
            Direction.East,
            Direction.South,
            Direction.West,
            Direction.North
        };

        public static Position GetOffset(this Direction dir)
        {
            var offset = Position.Zero;
            if ((dir & Direction.East)  == Direction.East)  offset += Position.UnitX;
            if ((dir & Direction.South) == Direction.South) offset += Position.UnitY;
            if ((dir & Direction.West)  == Direction.West)  offset -= Position.UnitX;
            if ((dir & Direction.North) == Direction.North) offset -= Position.UnitY;
            return offset;
        }

        public static double CurTime()
        {
            return SWatch.Elapsed.TotalSeconds;
        }

        public static Direction Left(this Direction dir)
        {
            int val = (int) dir;
            return (Direction) (((val << 3) | (val >> 1)) & 0xf);
        }

        public static Direction Right(this Direction dir)
        {
            int val = (int) dir;
            return (Direction) (((val << 1) | (val >> 3)) & 0xf);
        }

        public static bool HasElement(this XElement elem, XName name)
        {
            return elem.Elements(name).Count() > 0;
        }

        public static bool HasAttribute(this XElement elem, XName name)
        {
            return elem.Attributes(name).Count() > 0;
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

        public static float NextFloat(this Random rand)
        {
            return (float) rand.NextDouble();
        }

        public static float NextFloat(this Random rand, float max)
        {
            return (float) rand.NextDouble() * max;
        }

        public static float NextFloat(this Random rand, float min, float max)
        {
            return min + (float) rand.NextDouble() * (max - min);
        }

        private class NodeInfo<T>
        {
            private float _heuristic;

            public T Node { get; private set; }
            public Position Pos { get; private set; }
            public NodeInfo<T> Prev { get; private set; }
            public int Depth { get; private set; }
            public float Cost { get; private set; }
            public float Total { get; private set; }

            public float Heuristic
            {
                get { return _heuristic; }
                set
                {
                    _heuristic = value;
                    Total = Cost + value;
                }
            }

            public NodeInfo(T node, Position pos)
            {
                Node = node;
                Pos = pos;
                Prev = null;
                Depth = 0;
                Cost = 0f;
            }

            public NodeInfo(T node, Position pos, NodeInfo<T> prev, float costAdd)
            {
                Node = node;
                Pos = pos;
                Prev = prev;
                Depth = prev.Depth + 1;
                Cost = prev.Cost + costAdd;
            }

            public void CalculateHeuristic(Position target)
            {
                Heuristic = (target - Pos).ManhattanLength;
            }
        }

        public static T[] AStar<T>(T origin, T target,
            Func<T, IEnumerable<Tuple<T, int>>> adjFunc, Func<T, Position> posFunc)
        {
            var open = new List<NodeInfo<T>>();
            var clsd = new HashSet<NodeInfo<T>>();

            var targPos = posFunc(target);

            var first = new NodeInfo<T>(origin, posFunc(origin));
            first.CalculateHeuristic(targPos);

            open.Add(first);

            while (open.Count > 0) {
                NodeInfo<T> cur = null;
                foreach (var node in open) {
                    if (cur == null || node.Total < cur.Total) cur = node;
                }

                if (cur.Node.Equals(target)) {
                    var path = new T[cur.Depth + 1];
                    for (int i = cur.Depth; i >= 0; --i) {
                        path[i] = cur.Node;
                        cur = cur.Prev;
                    }
                    return path;
                }

                open.Remove(cur);
                clsd.Add(cur);

                foreach (var adj in adjFunc(cur.Node)) {
                    var node = new NodeInfo<T>(adj.Item1, posFunc(adj.Item1), cur, adj.Item2);
                    var existing = clsd.FirstOrDefault(x => x.Node.Equals(adj.Item1));

                    if (existing != null) {
                        if (existing.Cost <= node.Cost) continue;

                        clsd.Remove(existing);
                        node.Heuristic = existing.Heuristic;
                    }

                    existing = open.FirstOrDefault(x => x.Node.Equals(adj.Item1));

                    if (existing != null) {
                        if (existing.Cost <= node.Cost) continue;

                        open.Remove(existing);
                        node.Heuristic = existing.Heuristic;
                    } else {
                        node.CalculateHeuristic(targPos);
                    }

                    open.Add(node);
                }
            }

            return null;
        }
    }
}
