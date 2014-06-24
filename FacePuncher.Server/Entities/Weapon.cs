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
    class Weapon : Component
    {
        class AttackClass : IDefinitionLoadable
        {
            [ScriptDefinable]
            public int BaseDamage { get; set; }

            [ScriptDefinable]
            public float BaseDuration { get; set; }

            [ScriptDefinable]
            public float Effort { get; set; }

            [ScriptDefinable]
            public float Precision { get; set; }

            [ScriptDefinable]
            public float Range { get; set; }

            [ScriptDefinable]
            public String DamageType { get; set; }

            public void LoadFromDefinition(System.Xml.Linq.XElement elem)
            {
                Definitions.LoadProperties(this, elem);
            }
        }

        private Dictionary<String, float> _skills;
        private Dictionary<String, AttackClass> _attacks;

        public Weapon()
        {
            _skills = new Dictionary<string, float>();
            _attacks = new Dictionary<string, AttackClass>();
        }
        
        public override void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            base.LoadFromDefinition(elem);

            elem.Elements("Skill").LoadDictionary(_skills,
                x => float.Parse(x.Attribute("weight").Value));

            elem.Elements("Attack").LoadDictionary(_attacks);
        }

        public Delay Attack(String attack, Entity target)
        {
            return Attack(attack, Entity.Parent, target);
        }

        public Delay Attack(String attack, Entity attacker, Entity target)
        {
            var atk = _attacks[attack];
            var dmg = new DamageInfo(atk.BaseDamage) {
                Attacker = attacker,
                Inflictor = Entity
            };

            var mat = Entity.GetComponentOrNull<Material>();

            if (mat != null) {
                mat.ModifyDamage(dmg);
            }

            dmg.Set("Material", mat);
            dmg.Set("DamageType", atk.DamageType);

            var damageable = target.GetComponentOrNull<Damageable>();

            if (damageable == null) {
                return Delay(atk.BaseDuration);
            }

            if ((target.Position - attacker.Position).LengthSquared > atk.Range * atk.Range) {
                return Delay(atk.BaseDuration);
            }

            damageable.Damage(dmg);

            return Delay(atk.BaseDuration);
        }
    }
}
