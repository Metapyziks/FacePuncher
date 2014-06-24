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

namespace FacePuncher.Entities
{
    sealed class DamageInfo
    {
        public float DamageMultiplier { get; set; }

        public int BaseDamage { get; set; }

        public int MaxDamage { get; set; }

        public int ScaledDamage
        {
            get
            {
                return Tools.Clamp((int) Math.Round(BaseDamage * DamageMultiplier), 0, MaxDamage);
            }
        }

        public Entity Attacker { get; set; }

        public Entity Inflictor { get; set; }

        private Dictionary<String, Object> _keyVals;

        public DamageInfo(int baseDamage)
        {
            DamageMultiplier = 1f;

            BaseDamage = baseDamage;
            MaxDamage = int.MaxValue;

            _keyVals = new Dictionary<string, object>();
        }

        public void Set<T>(String key, T val)
        {
            if (!_keyVals.ContainsKey(key)) {
                _keyVals.Add(key, val);
            } else {
                _keyVals[key] = val;
            }
        }

        public T Get<T>(String key, T defaultVal = default(T))
        {
            if (!_keyVals.ContainsKey(key)) {
                return defaultVal;
            } else {
                return (T) _keyVals[key];
            }
        }

        public void Add(int damage)
        {
            if (damage < 0) Subtract(-damage);
            else BaseDamage += damage;
        }

        public void Subtract(int damage)
        {
            if (damage < 0) Add(-damage);
            else BaseDamage = Math.Max(0, BaseDamage - damage);
        }

        public void Scale(float scale)
        {
            DamageMultiplier *= Math.Max(0, scale);
        }
    }

    class DamagedEventArgs : EventArgs
    {
        public Entity Victim { get; private set; }

        public DamageInfo Damage { get; private set; }

        public DamagedEventArgs(Entity victim, DamageInfo info)
        {
            Victim = victim;
            Damage = info;
        }
    }

    class Damageable : Component
    {
        private int _maxHealth;

        [ScriptDefinable]
        public int MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                _maxHealth = value;
                Health = Math.Min(Health, _maxHealth);
            }
        }

        public int Health { get; private set; }

        public bool IsAlive { get { return Health > 0; } }

        public event EventHandler<DamagedEventArgs> Damaged;

        public event EventHandler<DamagedEventArgs> Killed;

        public Damageable()
        {
            MaxHealth = 1;
        }

        public override void LoadFromDefinition(XElement elem)
        {
            base.LoadFromDefinition(elem);

            Health = MaxHealth;
        }

        public void Damage(int baseDamage)
        {
            Damage(new DamageInfo(baseDamage));
        }

        public void Damage(DamageInfo info)
        {
            if (Damaged != null) {
                Damaged(this, new DamagedEventArgs(Entity, info));
            }
            
            if (!IsAlive) return;

            Health = Math.Max(0, Health - info.ScaledDamage);

            if (!IsAlive && Killed != null) {
                Killed(this, new DamagedEventArgs(Entity, info));
            }
        }
    }
}
