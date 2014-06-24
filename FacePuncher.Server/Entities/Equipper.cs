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
    class Equipper : Component
    {
        private Dictionary<String, Entity> _slots;

        public Equipper()
        {
            _slots = new Dictionary<string, Entity>();
        }

        public Delay Equip(Entity item, String[] slots)
        {
            if (_slots.Values.All(x => x == item)) return Delay(0);

            var time = 0.0;

            var cur = slots
                .Where(x => HasEquipped(x))
                .Select(x => GetEquipped(x))
                .Distinct()
                .ToArray();

            foreach (var itm in cur) {
                time += Unequip(itm).Delta;
            }

            var equp = item.GetComponent<Equipable>();

            foreach (var slot in slots) {
                _slots[slot] = item;
            }

            return Delay(time + equp.EquipTime);
        }

        public Delay Unequip(Entity item)
        {
            if (!_slots.ContainsValue(item)) return Delay(0);

            var equp = item.GetComponent<Equipable>();

            foreach (var slot in _slots.Keys) {
                _slots[slot] = null;
            }

            return Delay(equp.UnequipTime);
        }

        public bool HasEquipped(String slot)
        {
            if (_slots[slot] == null) return false;

            var item = _slots[slot];
            if (item.Parent != Entity) {
                _slots[slot] = null;
                return false;
            }

            return true;
        }

        public Entity GetEquipped(String slot)
        {
            if (!HasEquipped(slot)) return null;
            return _slots[slot];
        }

        public String[] GetSlotsOfEquipped(Entity item)
        {
            return _slots
                .Where(x => x.Value == item)
                .Select(x => x.Key)
                .ToArray();
        }
    }
}
