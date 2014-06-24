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

namespace FacePuncher.Entities
{
    class Container : Component
    {
        public IEnumerable<Entity> Items
        {
            get
            {
                return Entity.Children
                    .Where(x => x.HasComponent<InventoryItem>());
            }
        }

        public float Weight
        {
            get
            {
                return Items
                    .Sum(x => x.GetComponent<InventoryItem>().Weight);
            }
        }

        [ScriptDefinable]
        public float MaxWeight { get; set; }

        public bool CanAddItem(Entity item)
        {
            if (Entity.Children.Contains(item)) return false;

            var inv = item.GetComponentOrNull<InventoryItem>();
            return inv != null && Weight + inv.Weight <= MaxWeight;
        }

        public void AddItem(Entity item)
        {
            if (!CanAddItem(item)) throw new Exception("Cannot add item to container.");

            Entity.AddChild(item);
        }
    }
}
