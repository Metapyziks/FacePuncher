using System;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    class LevelGenerator : Generator<LevelGenerator>
    {
        static GeneratorCollection<LevelGenerator> _generators;

        static LevelGenerator()
        {
            _generators = new GeneratorCollection<LevelGenerator>();

            Definitions.RegisterType("level", _generators.Add);
        }

        public static Level Generate(String type, int seed = 0)
        {
            var generator = _generators[type];
            var rand = seed == 0 ? new Random() : new Random(seed);

            var level = new Level();

            generator.RoomPlacement.PlaceRooms(level, rand);

            return level;
        }
                    
        public RoomPlacement RoomPlacement { get; private set; }

        protected override void OnLoadFromDefinition(XElement elem)
        {
            RoomPlacement = LoadWorkerFromDefinition<RoomPlacement>(elem,
                RoomPlacement ?? new RoomPlacements.Default());
        }
    }
}
