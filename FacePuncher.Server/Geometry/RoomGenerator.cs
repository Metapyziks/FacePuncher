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

        public static RoomGenerator Get(String type)
        {
            return _generators[type];
        }

        public RoomLayout RoomLayout { get; set; }

        public ItemPlacement ItemPlacement { get; set; }

        protected override void OnLoadFromDefinition(XElement elem)
        {
            base.OnLoadFromDefinition(elem);

            RoomLayout = LoadWorkerFromDefinition<RoomLayout>(elem,
                RoomLayout ?? new RoomLayouts.Default());

            ItemPlacement = LoadWorkerFromDefinition<ItemPlacement>(elem,
                ItemPlacement ?? new ItemPlacements.Default());
        }

        public IEnumerable<Room> Generate(Level level, Rectangle rect, Rectangle[] doors, Random rand)
        {
            return RoomLayout.Generate(level, rect, doors, rand);
        }
    }
}
