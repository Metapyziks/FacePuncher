using System;
using System.Collections.Generic;

namespace FacePuncher.Geometry
{
    abstract class RoomLayout : GenerationWorker
    {
        public abstract IEnumerable<Room> Generate(Level level, Rectangle rect, Rectangle[] doors, Random rand);
    }
}
