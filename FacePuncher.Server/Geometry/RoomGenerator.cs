using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    class RoomGenerator : Generator<RoomGenerator>
    {
        static GeneratorCollection<RoomGenerator> _generators;

        static RoomGenerator()
        {
            _generators = new GeneratorCollection<RoomGenerator>();

            Definitions.RegisterType("room", _generators.Add);
        }

        public static void Generate(Level level, String type, Rectangle rect, Rectangle[] doors, Random rand)
        {
            var generator = _generators[type];
            var rooms = generator.RoomLayout.Generate(level, rect, doors, rand);
        }

        public RoomLayout RoomLayout { get; set; }

        public ItemPlacement ItemPlacement { get; set; }

        protected override void OnLoadFromDefinition(XElement elem)
        {
            RoomLayout = LoadWorkerFromDefinition<RoomLayout>(elem,
                RoomLayout ?? new RoomLayouts.Default());

            ItemPlacement = LoadWorkerFromDefinition<ItemPlacement>(elem,
                ItemPlacement ?? new ItemPlacements.Default());
        }
    }
}
