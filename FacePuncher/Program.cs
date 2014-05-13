using System;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var level = new Level();
            var room = level.CreateRoom(new Rectangle(4, 4, 12, 12));
            room.AddGeometry(new Rectangle(1, 1, room.Width - 2, room.Height - 2));
            room.SubtractGeometry(new Rectangle(2, 2, room.Width - 4, room.Height - 4));

            Display.Initialize(96, 32);
            level.Draw(Display.Rect, Position.Zero);
            Display.Refresh();

            Console.ReadKey();
        }
    }
}
