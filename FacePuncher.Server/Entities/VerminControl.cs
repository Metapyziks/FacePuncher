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
using System.Linq;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Component for vermin entities that should wander around randomly.
    /// </summary>
    class VerminControl : AgentControl
    {
        [ScriptDefinable]
        public double MinMovePeriod { get; set; }

        [ScriptDefinable]
        public double MaxMovePeriod { get; set; }
        
        public override void LoadFromDefinition(System.Xml.Linq.XElement elem)
        {
            base.LoadFromDefinition(elem);

            MovePeriod = Tools.Random.NextDouble() * (MaxMovePeriod - MinMovePeriod) + MinMovePeriod;
        }

        public override void OnPlace()
        {
            Entity.Wake();
        }

        public override async void OnWake()
        {
            while (IsActive) {
                var valid = Tools.Directions.Where(x => Entity.CanMove(x)).ToArray();
                await Move(valid[Tools.Random.Next(valid.Length)]);
            }
        }
    }
}
