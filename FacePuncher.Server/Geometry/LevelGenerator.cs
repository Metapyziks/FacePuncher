/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
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

        public static LevelGenerator Get(String type)
        {
            return _generators[type];
        }

        public RoomPlacement RoomPlacement { get; private set; }

        protected override void OnLoadFromDefinition(XElement elem)
        {
            RoomPlacement = LoadWorkerFromDefinition<RoomPlacement>(elem,
                RoomPlacement ?? new RoomPlacements.Default());
        }

        public Level Generate(int seed = 0)
        {
            var rand = seed == 0 ? new Random() : new Random(seed);
            var level = new Level();

            var plans = RoomPlacement.Generate(level, rand);

            foreach (var plan in plans) {
                plan.Generator.Generate(level, plan.Rect, plan.Doors.Values.ToArray(), rand);
            }

            return level;
        }
    }
}
