﻿/* Copyright (c) 2014 James King [metapyziks@gmail.com]
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
using System.Net.Sockets;
using System.Threading.Tasks;

using FacePuncher.Graphics;
using FacePuncher.Network;

namespace FacePuncher
{
    public enum InventoryAction
    {
        None = 0,
        Pickup,
        Drop,
        Equip,
        Remove
    }

    public class InventoryEntry
    {
        public static async Task<InventoryEntry> Read(NetworkStream stream)
        {
            var result = new InventoryEntry {
                ID = await stream.ReadUInt32(),
                Name = await stream.ReadString(),
                Weight = await stream.ReadSingle(),
                Value = await stream.ReadInt32()
            };

            var flags = await stream.ReadByteAsync();

            if ((flags & 0x01) != 0) {
                result.Appearance = await EntityAppearance.Read(stream);
            }

            if ((flags & 0x02) != 0) {
                result.Material = await stream.ReadString();
            }

            return result;
        }

        public uint ID { get; set; }

        public String Name { get; set; }
        public float Weight { get; set; }
        public int Value { get; set; }

        public String Material { get; set; }
        public EntityAppearance Appearance { get; set; }

        public void WriteToStream(NetworkStream stream)
        {
            stream.Write(ID);
            stream.Write(Name);

            stream.Write(Weight);
            stream.Write(Value);

            stream.Write((byte) (
                (Appearance != null ? 0x01 : 0x00) |
                (Material != null ? 0x02 : 0x00)));

            if (Appearance != null) {
                Appearance.WriteToStream(stream);
            }

            if (Material != null) {
                stream.Write(Material);
            }
        }
    }

    public class InventoryListing : IEnumerable<InventoryEntry>
    {
        public static async Task<InventoryListing> Read(NetworkStream stream)
        {
            var result = new InventoryListing((InventoryAction) await stream.ReadByteAsync());
            int count = await stream.ReadInt32();
            result.Offset = await stream.ReadInt32();
            result.Total = await stream.ReadInt32();
            for (int i = 0; i < count; ++i) {
                result._entries.Add(await InventoryEntry.Read(stream));
            }
            return result;
        }

        private List<InventoryEntry> _entries;

        public InventoryAction AllowedAction { get; set; }

        public int Count
        {
            get { return _entries.Count; }
        }

        public int Offset { get; set; }

        public int Total { get; set; }

        public InventoryEntry this[int index]
        {
            get { return _entries[index]; }
            set
            {
                _entries[index] = value;
            }
        }

        public InventoryListing(InventoryAction action)
        {
            AllowedAction = action;
            _entries = new List<InventoryEntry>();
        }

        public void Add(uint id, String name, float weight, int value,
            EntityAppearance appearance = null, String material = null)
        {
            _entries.Add(new InventoryEntry {
                ID = id,
                Name = name,
                Weight = weight,
                Value = value,
                Appearance = appearance,
                Material = material
            });
        }

        public IEnumerator<InventoryEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        public void WriteToStream(NetworkStream stream)
        {
            stream.Write((byte) AllowedAction);
            stream.Write(Count);
            stream.Write(Offset);
            stream.Write(Total);

            foreach (var entry in this) {
                entry.WriteToStream(stream);
            }
        }
    }
}
