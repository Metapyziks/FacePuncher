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

namespace FacePuncher
{
    public enum Interaction : byte
    {
        None = 0,
        PickupItem = 1
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(MoveIntent))]
    [ProtoInclude(2, typeof(InteractIntent))]
    public abstract class Intent
    {
        public static bool HandleIntent<THandled>(ref Intent intent, Action<THandled> handler) where THandled : Intent
        {
            var castIntent = intent as THandled;
            if (castIntent != null)
            {
                handler(castIntent);
                intent = null;
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
