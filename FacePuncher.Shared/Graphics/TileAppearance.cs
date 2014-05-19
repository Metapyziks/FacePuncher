/* Copyright (C) 2014 James King (metapyziks@gmail.com)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FacePuncher.Geometry;
using FacePuncher.Network;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FacePuncher.Graphics
{
    public class TileAppearance : IEnumerable<EntityAppearance>
    {
        public Position Position { get; private set; }

        public char Symbol { get; set; }

        public ConsoleColor ForeColor { get; set; }

        public ConsoleColor BackColor { get; set; }

        public EntityAppearance[] Entities { get; set; }

        public int EntityCount { get { return Entities.Length; } }
        
        public EntityAppearance this[int index]
        {
            get { return Entities[index]; }
        }

        public TileAppearance(Position pos)
        {
            Position = pos;

            Symbol = '?';
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Entities = new EntityAppearance[0];
        }

        public static async Task<TileAppearance> Read(Position pos, NetworkStream stream)
        {
            var appearance = await stream.ReadAppearance();

            var entities = new EntityAppearance[await stream.ReadByteAsync()];
            for (int i = 0; i < entities.Length; ++i) {
                entities[i] = await EntityAppearance.Read(stream);
            }

            return new TileAppearance(pos)
            {
                //TODO: this is a bit hacky
                Symbol = appearance.Item1,
                ForeColor = appearance.Item2,
                BackColor = appearance.Item3,
                Entities = entities
            };
        }

        public void WriteToStream(Stream stream)
        {
            if (EntityCount >= 256) {
                throw new Exception("Entity count on one tile larger than 255. If this "
                    + "ever happens naturally use something larger than a byte to send "
                    + "entity counts in TileAppearance.");
            }

            stream.WriteAppearance(Symbol, ForeColor, BackColor);
            stream.WriteByte((byte) EntityCount);

            foreach (var ent in this) {
                ent.WriteToStream(stream);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TileAppearance && Equals((TileAppearance) obj);
        }

        public bool Equals(TileAppearance that)
        {
            return this.Position == that.Position
                && this.Symbol == that.Symbol
                && this.ForeColor == that.ForeColor
                && this.BackColor == that.BackColor
                && this.EntityCount == that.EntityCount
                && this.Entities.Zip(that, (a, b) => a.Equals(b)).All(x => x);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode()
                 ^ EntityCount << 16;
        }

        public IEnumerator<EntityAppearance> GetEnumerator()
        {
            return Entities.AsEnumerable<EntityAppearance>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Entities.GetEnumerator();
        }
    }
}
