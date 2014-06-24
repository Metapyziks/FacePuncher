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

namespace FacePuncher.Entities
{
    interface IWeighable
    {
        float Weight { get; }
    }

    interface IValuable
    {
        float Value { get; }
    }

    class InventoryItem : Component
    {
        [ScriptDefinable]
        public float BaseWeight { get; set; }

        [ScriptDefinable]
        public float BaseValue { get; set; }

        public float Weight
        {
            get
            {
                var mul = 1f;
                foreach (var comp in Entity) {
                    if (comp is IWeighable) {
                        mul *= ((IWeighable) comp).Weight;
                    }
                }

                return BaseWeight * mul;
            }
        }

        public int Value
        {
            get
            {
                var mul = 1f;
                foreach (var comp in Entity) {
                    if (comp is IValuable) {
                        mul *= ((IValuable) comp).Value;
                    }
                }

                return (int) Math.Round(BaseValue * mul);
            }
        }
    }
}
