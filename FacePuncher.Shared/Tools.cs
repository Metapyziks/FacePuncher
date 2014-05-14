using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using FacePuncher.Geometry;

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

        public static bool HasElement(this XElement elem, string name)
        {
            return elem.Elements(name).Count() > 0;
        }

        public static T Element<T>(this XElement elem, string name)
            where T : struct
        {
            if (typeof(T).IsEnum) {
                return (T) Enum.Parse(typeof(T), elem.Element(name).Value, true);
            } else {
                return (T) Convert.ChangeType(elem.Element(name).Value, typeof(T));
            }
        }

        public static int Clamp(this int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }

        public static void Write(this BinaryWriter writer, Position pos)
        {
            writer.Write(pos.X);
            writer.Write(pos.Y);
        }

        public static Position ReadPosition(this BinaryReader reader)
        {
            return new Position(
                reader.ReadInt32(),
                reader.ReadInt32());
        }

        public static void Write(this BinaryWriter writer, Rectangle rect)
        {
            writer.Write(rect.Left);
            writer.Write(rect.Top);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }

        public static Rectangle ReadRectangle(this BinaryReader reader)
        {
            return new Rectangle(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());
        }
    }
}
