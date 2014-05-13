using System;

using FacePuncher.Entities;
using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Entity.Register("player", ent => {
                ent.AddComponent<PlayerControlTest>();
                ent.AddComponent<StaticDrawable>()
                    .SetSymbol('@')
                    .SetForeColor(ConsoleColor.Yellow);
            });

            var level = new Level();
            var room = level.CreateRoom(new Rectangle(4, 4, 12, 12));
            room.AddGeometry(new Rectangle(1, 1, room.Width - 2, room.Height - 2));
            room.SubtractGeometry(new Rectangle(2, 2, room.Width - 4, room.Height - 4));

            var ply = Entity.Create("player");
            ply.Place(room[4, 4]);

            Display.Initialize(96, 32);

            for (ulong time = 0; ; ++time) {
                level.Think(time);
                level.Draw(Display.Rect, Position.Zero, new DrawAttributes(0));
                
                Display.Refresh();
            }

            Console.ReadKey();
        }
    }
}
