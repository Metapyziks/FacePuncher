using System.IO;

using FacePuncher.Geometry;

namespace FacePuncher
{
    public static class Tools
    {
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
