using System;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    abstract class RoomPlacement : GenerationWorker
    {
        public abstract void PlaceRooms(Level level, Random rand);
    }
}
