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
            var rand = new Random();

            Entity.Register("player", ent => {
                ent.AddComponent<PlayerControlTest>();
                ent.AddComponent<StaticDrawable>()
                    .SetLayer(DrawableLayer.Characters)
                    .SetSymbol('@')
                    .SetForeColor(ConsoleColor.Yellow);
            });

            var dustSymbols = new[] {
                ',', '.', '`', '\''
            };

            Entity.Register("dust", ent => {
                ent.AddComponent<StaticDrawable>()
                    .SetLayer(DrawableLayer.Debris)
                    .SetSymbol(dustSymbols[rand.Next(dustSymbols.Length)])
                    .SetForeColor(ConsoleColor.DarkGray);
            });

            var level = new Level();
            var room = level.CreateRoom(new Rectangle(4, 4, 12, 12));
            room.AddGeometry(new Rectangle(1, 1, room.Width - 2, room.Height - 2));
            room.SubtractGeometry(new Rectangle(2, 2, room.Width - 4, room.Height - 4));
            room.AddGeometry(new Rectangle(6, 7, 4, 1));
            room.AddGeometry(new Rectangle(11, 7, 2, 4));
            room.SubtractGeometry(new Rectangle(10, 8, 2, 2));

            var ply = Entity.Create("player");
            ply.Place(room[4, 4]);

            room = level.CreateRoom(new Rectangle(room.Right, room.Top + 7, 12, 4));
            room.AddGeometry(room.RelativeRect);
            room.SubtractGeometry(new Rectangle(0, 1, room.Width - 1, 2));

            foreach (var rm in level) {
                foreach (var tile in rm) {
                    if (rand.NextDouble() < 0.125 && tile.State == TileState.Floor) {
                        var dust = Entity.Create("dust");
                        dust.Place(tile);
                    }
                }
            }
            
            Display.Initialize(96, 32);

            for (ulong time = 0; ; ++time) {
                level.Think(time);
                level.Draw(Display.Rect, Position.Zero, new DrawAttributes(0));
                
                Display.Refresh();
            }
        }
    }
}
