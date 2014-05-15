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

        public static readonly Dictionary<ConsoleKey, Direction> MovementKeys
            = new Dictionary<ConsoleKey, Direction> {
            { ConsoleKey.NumPad7, Direction.NorthWest },
            { ConsoleKey.NumPad8, Direction.North },
            { ConsoleKey.UpArrow, Direction.North },
            { ConsoleKey.NumPad9, Direction.NorthEast },
            { ConsoleKey.NumPad4, Direction.West },
            { ConsoleKey.LeftArrow, Direction.West },
            { ConsoleKey.NumPad5, Direction.None },
            { ConsoleKey.NumPad6, Direction.East },
            { ConsoleKey.RightArrow, Direction.East },
            { ConsoleKey.NumPad1, Direction.SouthWest },
            { ConsoleKey.NumPad2, Direction.South },
            { ConsoleKey.DownArrow, Direction.South },
            { ConsoleKey.NumPad3, Direction.SouthEast }
        };

        public static readonly Direction[] Directions = new[] {
            Direction.NorthWest, Direction.North, Direction.NorthEast,
            Direction.West, Direction.East,
            Direction.SouthWest, Direction.South, Direction.SouthEast
        };

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
                await stream.ReadInt32(),
                await stream.ReadInt32());
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
                await stream.ReadInt32(),
                await stream.ReadInt32(),
                await stream.ReadInt32(),
                await stream.ReadInt32());
        }
    }
}
