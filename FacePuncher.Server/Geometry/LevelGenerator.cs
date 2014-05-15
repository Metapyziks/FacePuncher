using System;

using FacePuncher.Entities;

namespace FacePuncher.Geometry
{
    class LevelGenerator
    {
        public Level Generate(int seed)
        {
            // TODO: Temporary generator, will be gutted and replaced soon.

            var rand = new Random(seed == 0 ? (int) (DateTime.Now.Ticks & 0x7fffffff) : seed);

            var level = new Level();

            var debris = Entity.GetClassNames("dust", true);

            for (int i = 0; i < 4; ++i) {
                for (int j = 0; j < 4; ++j) {
                    var room = level.CreateRoom(new Rectangle(i * 8, j * 8, 8, 8));
                    
                    room.CreateWall(new Rectangle(0, 0, room.Width, room.Height));
                    room.CreateFloor(new Rectangle(1, 1, room.Width - 2, room.Height - 2));

                    if (i > 0) room.CreateFloor(new Rectangle(0, 3, 1, 2));
                    if (j > 0) room.CreateFloor(new Rectangle(3, 0, 2, 1));
                    if (i < 3) room.CreateFloor(new Rectangle(7, 3, 1, 2));
                    if (j < 3) room.CreateFloor(new Rectangle(3, 7, 2, 1));

                    foreach (var tile in room) {
                        if (tile.State == TileState.Floor) {
                            if (rand.NextDouble() < 1 / 4.0) {
                                var dust = Entity.Create(debris[rand.Next(debris.Length)]);
                                dust.Place(tile);
                            } else if (rand.NextDouble() < 1 / 128.0) {
                                var vermin = Entity.Create("vermin");
                                vermin.Place(tile);
                            }
                        }
                    }
                }
            }

            return level;
        }
    }
}
