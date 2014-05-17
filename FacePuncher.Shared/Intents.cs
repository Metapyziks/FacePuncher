/* Copyright (C) 2014 Tamme Schichler (tammeschichler@googlemail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using FacePuncher.Geometry;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher
{
    [ProtoContract]
    [ProtoInclude(1, typeof(MoveIntent))]
    public abstract class Intent
    {
        public static void HandleIntent<THandled>(ref Intent intent, Func<THandled, bool> handler) where THandled : Intent
        {
            var castIntent = intent as THandled;
            if (castIntent != null)
            {
                if (handler(castIntent))
                { intent = null; }
            }
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
}
