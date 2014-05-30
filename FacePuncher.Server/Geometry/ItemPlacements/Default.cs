/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FacePuncher.Geometry.ItemPlacements
{
    class Default : ItemPlacement
    {
        private Dictionary<ItemGenerator, int> _itemGenerators;

        [ScriptDefinable]
        public float MinFrequency { get; set; }

        [ScriptDefinable]
        public float MaxFrequency { get; set; }

        public Default()
        {
            MinFrequency = 0;
            MaxFrequency = 0;
        }

        public override void LoadFromDefinition(XElement elem)
        {
            base.LoadFromDefinition(elem);

            _itemGenerators = elem.Elements("Item")
                .ToFrequencyDictionary(x => ItemGenerator.Get(x));
        }

        public override void Generate(IEnumerable<Room> rooms, Random rand)
        {
            var tiles = rooms
                .SelectMany(x => x
                    .Where(y => y.State == TileState.Floor))
                .ToArray();

            int count = (int) Math.Round(rand.NextFloat(MinFrequency, MaxFrequency) * tiles.Length);

            tiles = tiles
                .OrderBy(x => rand.Next())
                .Take(count)
                .ToArray();

            foreach (var tile in tiles) {
                _itemGenerators.SelectRandom(rand).Generate(tile, rand);
            }
        }
    }
}
