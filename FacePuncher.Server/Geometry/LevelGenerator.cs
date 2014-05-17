/* Copyright (C) 2014 James King (metapyziks@gmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

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

            generator.RoomPlacement.Generate(level, rand);

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
