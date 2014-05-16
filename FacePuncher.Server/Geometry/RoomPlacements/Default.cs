using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FacePuncher.Geometry.RoomPlacements
{
    class Default : RoomPlacement
    {
        protected class RoomGeneratorInfo
        {
            public String ClassName { get; private set; }

            [ScriptDefinable]
            public int Frequency { get; set; }

            public void Initialize(XElement elem)
            {
                ClassName = elem.Attribute("class").Value;

                Definitions.LoadProperties(this, elem);
            }
        }

        private List<RoomGeneratorInfo> _roomGenerators;

        [ScriptDefinable]
        public int MinimumArea { get; set; }

        [ScriptDefinable]
        public int MaximumArea { get; set; }

        protected IEnumerable<RoomGeneratorInfo> RoomGenerators { get { return _roomGenerators; } }

        public Default()
        {
            MinimumArea = 1000;
            MaximumArea = 1000;

            _roomGenerators = new List<RoomGeneratorInfo>();
        }

        public override void LoadFromDefinition(XElement elem)
        {
            base.LoadFromDefinition(elem);

            foreach (var room in elem.Elements("Room")) {
                var info = new RoomGeneratorInfo();
                info.Initialize(elem);
                _roomGenerators.Add(info);
            }
        }

        protected RoomGeneratorInfo GetRandomGenerator(Random rand)
        {
            int total = _roomGenerators.Sum(x => x.Frequency);
            int index = rand.Next(total);

            return _roomGenerators.First(x => (index -= x.Frequency) < 0);
        }

        public override void PlaceRooms(Level level, Random rand)
        {
            int destArea = rand.Next(MinimumArea, MaximumArea);

            while (destArea > 0) {
                var genName = GetRandomGenerator(rand).ClassName;
                var generator = RoomGenerator.Generate(genName, level,  rand);
            }
        }
    }
}
