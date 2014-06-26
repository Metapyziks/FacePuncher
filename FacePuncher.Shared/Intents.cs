/* Copyright (c) 2014 Tamme Schichler [tammeschichler@googlemail.com]
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

using ProtoBuf;

using FacePuncher.Geometry;
using System.Threading.Tasks;

namespace FacePuncher
{
    public enum Interaction : byte
    {
        None = 0,
        PickupItem = 1,
        ViewInventory = 2,
        EquipItem = 3,
        RemoveItem = 4
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(MoveIntent))]
    [ProtoInclude(2, typeof(InteractIntent))]
    public abstract class Intent
    {
        private bool _handled = false;

        public static bool HandleIntent<THandled>(Intent intent, Action<THandled> handler) where THandled : Intent
        {
            var castIntent = intent as THandled;
            if (castIntent != null && !castIntent._handled) {
                castIntent._handled = true;
                handler(castIntent);
                return true;
            }

            return false;
        }

        public static async Task<bool> HandleIntentAsync<THandled>(Intent intent, Func<THandled, Task> handler) where THandled : Intent
        {
            var castIntent = intent as THandled;
            if (castIntent != null && !castIntent._handled) {
                castIntent._handled = true;
                await handler(castIntent);
                return true;
            }

            return false;
        }
    }

    [ProtoContract]
    public class MoveIntent : Intent
    {
        [ProtoMember(1)]
        public Direction Direction { get; set; }
        public MoveIntent() : this(Direction.None) { }
        public MoveIntent(Direction direction)
        { Direction = direction; }
    }

    [ProtoContract]
    public class InteractIntent : Intent
    {
        [ProtoMember(1)]
        public Interaction Interaction { get; set; }
        public InteractIntent() : this(Interaction.None) { }
        public InteractIntent(Interaction interaction)
        { Interaction = interaction; }
    }
}
