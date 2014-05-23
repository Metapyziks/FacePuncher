/* Copyright (c) 2014 James King [metapyziks@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Linq;

namespace FacePuncher.Entities
{
    /// <summary>
    /// Component for vermin entities that should wander around randomly.
    /// </summary>
    class VerminControl : AgentControl
    {
        public override void OnThink()
        {
            if (!CanMove || Tools.Random.NextDouble() < 0.5) return;

            var valid = Tools.Directions.Where(x => Entity.CanMove(x)).ToArray();
            Move(valid[Tools.Random.Next(valid.Length)]);
        }
    }
}
