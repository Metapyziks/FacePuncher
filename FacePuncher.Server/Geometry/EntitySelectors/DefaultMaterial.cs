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
using System.Xml.Linq;

using FacePuncher.Entities;

namespace FacePuncher.Geometry.EntitySelectors
{
    class DefaultMaterial : Default
    {
        private Dictionary<String, int> _materials;

        public DefaultMaterial()
        {
            _materials = new Dictionary<string, int>();
        }

        public override void LoadFromDefinition(XElement elem)
        {
            base.LoadFromDefinition(elem);

            _materials = elem.Elements("Material")
                .ToFrequencyDictionary(x => x);
        }

        public override Entity Select(Random rand)
        {
            var ent = base.Select(rand);
            var mat = ent.GetComponentOrNull<Material>();

            if (mat == null) {
                mat = ent.AddComponent<Material>();
            }

            if (_materials.Count > 0) {
                mat.ClassName = _materials.SelectRandom(rand);
            }

            return ent;
        }
    }
}
