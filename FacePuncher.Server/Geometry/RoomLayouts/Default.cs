using System;
using System.Collections.Generic;

namespace FacePuncher.Geometry.RoomLayouts
{
    class Default : RoomLayout
    {
        public override IEnumerable<Room> Generate(Level level, Rectangle rect, Rectangle[] doors, Random rand)
        {
            var room = level.CreateRoom(rect);
            rect -= rect.TopLeft;

            room.CreateWall(rect);
            room.CreateFloor(new Rectangle(1, 1, rect.Width - 2, rect.Height - 2));

            foreach (var door in doors) room.CreateFloor(door);

            return new[] { room };
        }
    }
}
