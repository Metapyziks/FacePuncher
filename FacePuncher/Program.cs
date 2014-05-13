using System;
using System.Threading;

using FacePuncher.Entities;
using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher
{
    class Program
    {
        static Level Generate(int seed)
        {
            var rand = new Random(seed == 0 ? (int) (DateTime.Now.Ticks & 0x7fffffff) : seed);

            var level = new Level();

            for (int i = 0; i < 4; ++i) {
                for (int j = 0; j < 4; ++j) {
                    var room = level.CreateRoom(new Rectangle(i * 8, j * 8, 8, 8));

                    room.AddGeometry(new Rectangle(0, 0, room.Width, room.Height));
                    room.SubtractGeometry(new Rectangle(1, 1, room.Width - 2, room.Height - 2));

                    if (i > 0) room.SubtractGeometry(new Rectangle(0, 3, 1, 2));
                    if (j > 0) room.SubtractGeometry(new Rectangle(3, 0, 2, 1));
                    if (i < 3) room.SubtractGeometry(new Rectangle(7, 3, 1, 2));
                    if (j < 3) room.SubtractGeometry(new Rectangle(3, 7, 2, 1));

                    foreach (var tile in room) {
                        if (tile.State == TileState.Floor && rand.NextDouble() < 0.125) {
                            var dust = Entity.Create("dust");
                            dust.Place(tile);
                        }
                    }
                }
            }

            return level;
        }

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

            var level = Generate(0);

            var ply = Entity.Create("player");
            ply.Place(level[4, 4]);
            
            Display.Initialize(96, 32);

            var halfSize = new Position(Display.Width / 2, Display.Height / 2);

            int flash = 0;
            var renderTimer = new Timer(state => {
                Display.Clear();
                lock (level) {
                    level.Draw(Display.Rect + ply.Position - halfSize,
                        Position.Zero, new DrawAttributes(ply.Position, flash++));
                }
                Display.Refresh();
            }, null, 0, 125);
            
            Console.ReadKey(true);
        }
    }
}
