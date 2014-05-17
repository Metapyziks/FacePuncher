using System;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    abstract class RoomPlacement : GenerationWorker
    {
        public abstract void Generate(Level level, Random rand);
    }
}
