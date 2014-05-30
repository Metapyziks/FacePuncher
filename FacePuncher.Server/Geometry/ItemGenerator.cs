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
using System.Xml.Linq;

namespace FacePuncher.Geometry
{
    class ItemGenerator : Generator<ItemGenerator>
    {
        static GeneratorCollection<ItemGenerator> _generators;

        static ItemGenerator()
        {
            _generators = new GeneratorCollection<ItemGenerator>();

            Definitions.RegisterType("item", _generators.Add);
        }

        public static ItemGenerator Get(String type)
        {
            return _generators[type];
        }

        public EntitySelector EntitySelector { get; private set; }

        protected override void OnLoadFromDefinition(XElement elem)
        {
            EntitySelector = LoadWorkerFromDefinition<EntitySelector>(elem,
                EntitySelector ?? new EntitySelectors.Default());
        }

        public void Generate(Tile tile, Random rand)
        {
            EntitySelector.Select(rand).Place(tile);
        }
    }
}
