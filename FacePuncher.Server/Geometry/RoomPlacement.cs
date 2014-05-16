using System;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    abstract class RoomPlacement
    {
        public virtual void LoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);
        }

        public abstract void PlaceRooms(Level level, Random rand);
    }
}
