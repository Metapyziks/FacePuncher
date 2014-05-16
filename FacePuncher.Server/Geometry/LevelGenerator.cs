using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using FacePuncher.Entities;

namespace FacePuncher.Geometry
{
    static class LevelGenerator
    {
        class LevelGeneratorInfo : GeneratorInfo
        {
            private RoomPlacement _roomPlacement;

            public LevelGeneratorInfo Base
            {
                get { return HasBase ? _generators[BaseName] : null; }
            }

            public RoomPlacement RoomPlacement
            {
                get { return _roomPlacement ?? Base.RoomPlacement; }
            }

            protected override void LoadFromDefinition(XElement elem)
            {
                if (elem.HasElement("RoomPlacement")) {
                    var sub = elem.Element("RoomPlacement");
                    var name = sub.Attribute("class").Value;
                    var typeName = String.Format("FacePuncher.Geometry.RoomPlacements.{0}", name);

                    var type = Assembly.GetEntryAssembly().GetType(typeName);
                    if (type == null) throw new Exception("Invalid RoomPlacement type specified.");

                    var ctor = type.GetConstructor(new Type[0]);
                    if (ctor == null) throw new Exception(String.Format("RoomPlacement type {0} has no valid constructor.", name));

                    _roomPlacement = (RoomPlacement) ctor.Invoke(new Object[0]);
                    _roomPlacement.LoadFromDefinition(sub);
                } else if (!HasBase) {
                    throw new Exception("No RoomPlacement specified in definition.");
                }
            }
        }

        static Dictionary<String, LevelGeneratorInfo> _generators;

        static LevelGenerator()
        {
            _generators = new Dictionary<string, LevelGeneratorInfo>();

            Definitions.RegisterType("level", elem => {
                var info = new LevelGeneratorInfo();
                info.Initialize(elem);

                _generators.Add(info.Name, info);
            });
        }

        public static Level Generate(String type, int seed = 0)
        {
            var info = _generators[type];
            var rand = seed == 0 ? new Random() : new Random(seed);

            var level = new Level();
            info.RoomPlacement.PlaceRooms(level, rand);

            return level;
        }
    }
}
