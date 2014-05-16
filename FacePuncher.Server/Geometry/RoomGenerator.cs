using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    class RoomGenerator
    {
        class RoomGeneratorInfo : GeneratorInfo
        {
            private RoomLayout _roomLayout;

            public RoomGeneratorInfo Base
            {
                get { return HasBase ? _generators[BaseName] : null; }
            }

            public RoomLayout RoomLayout
            {
                get { return _roomLayout ?? Base.RoomLayout; }
            }

            protected override void OnLoadFromDefinition(XElement elem)
            {
                throw new NotImplementedException();
            }
        }

        static Dictionary<String, RoomGeneratorInfo> _generators;

        public static IEnumerable<Room> Generate(String type, Level level, Rectangle rect, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
