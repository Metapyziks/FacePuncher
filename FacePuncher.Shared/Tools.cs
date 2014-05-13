using System;
using System.Collections.Generic;
using System.IO;
using FacePuncher.Geometry;

namespace FacePuncher
{
    public static class Tools
    {
        public static readonly Dictionary<ConsoleKey, Direction> MovementKeys
            = new Dictionary<ConsoleKey, Direction> {
            { ConsoleKey.NumPad7, Direction.NorthWest },
            { ConsoleKey.NumPad8, Direction.North },
            { ConsoleKey.UpArrow, Direction.North },
            { ConsoleKey.NumPad9, Direction.NorthEast },
            { ConsoleKey.NumPad4, Direction.West },
            { ConsoleKey.LeftArrow, Direction.West },
            { ConsoleKey.NumPad6, Direction.East },
            { ConsoleKey.RightArrow, Direction.East },
            { ConsoleKey.NumPad1, Direction.SouthWest },
            { ConsoleKey.NumPad2, Direction.South },
            { ConsoleKey.DownArrow, Direction.South },
            { ConsoleKey.NumPad3, Direction.SouthEast }
        };

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
