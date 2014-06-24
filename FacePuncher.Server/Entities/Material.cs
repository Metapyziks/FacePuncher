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

namespace FacePuncher.Entities
{
    class Material : IDefinitionLoadable
    {
        private static Dictionary<String, Material> _sMaterials;

        static Material()
        {
            _sMaterials = new Dictionary<string, Material>();

            Definitions.RegisterType("material", elem => {
                var name = elem.Attribute("name").Value;

                if (!_sMaterials.ContainsKey(name)) {
                    _sMaterials.Add(name, new Material());
                }

                _sMaterials[name].LoadFromDefinition(elem);
            });
        }

        [ScriptDefinable]
        public float Weight { get; set; }

        [ScriptDefinable]
        public float Durability { get; set; }

        [ScriptDefinable]
        public float Value { get; set; }

        [ScriptDefinable]
        public ConsoleColor Color { get; set; }

        private Dictionary<String, float> _damageModifiers;

        public Material()
        {
            _damageModifiers = new Dictionary<string, float>();
        }

        public void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            Definitions.LoadProperties(this, elem);

            foreach (var mod in elem.Elements("DamageModifier")) {
                var type = mod.Attribute("type").Value;
                var value = float.Parse(mod.Value);

                if (!_damageModifiers.ContainsKey(type) {
                    _damageModifiers.Add(type, value);
                } else {
                    _damageModifiers[type] = value;
                }
            }
        }

        public void ModifyDamage(DamageInfo info)
        {
            foreach (var type in info.DamageTypes) {
                if (_damageModifiers.ContainsKey(type)) {
                    info.Scale(_damageModifiers[type]);
                }
            }
        }
    }
}
