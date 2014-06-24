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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FacePuncher.Network
{
    public static class NetworkStreamExtensions
    {
        public static async Task<byte[]> ReadBytes(this NetworkStream stream, int count)
        {
            var result = new byte[count];
            var pos = 0;
            while (pos < count)
            {
                var read = await stream.ReadAsync(result, pos, count - pos);
                if (read == 0)
                {
                    // graceful TCP shutdown
                    throw new EndOfStreamException();
                }
                pos += read;
            }
            return result;
        }
        public static async Task<byte> ReadByteAsync(this NetworkStream stream)
        {
            return (await stream.ReadBytes(1))[0];
        }

        /// <summary>
        /// Converts to/from network byte order if necessary.
        /// </summary>
        private static byte[] Swivel(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
            {
                return input.Reverse().ToArray();
            }
            else return input.ToArray();
        }
        #region Reading
        public static async Task<Int32> ReadInt32(this NetworkStream stream) { return BitConverter.ToInt32(Swivel(await stream.ReadBytes(sizeof(Int32))), 0); }
        public static async Task<UInt16> ReadUInt16(this NetworkStream stream) { return BitConverter.ToUInt16(Swivel(await stream.ReadBytes(sizeof(UInt16))), 0); }
        public static async Task<UInt32> ReadUInt32(this NetworkStream stream) { return BitConverter.ToUInt32(Swivel(await stream.ReadBytes(sizeof(UInt32))), 0); }
        public static async Task<UInt64> ReadUInt64(this NetworkStream stream) { return BitConverter.ToUInt64(Swivel(await stream.ReadBytes(sizeof(UInt64))), 0); }
        public static async Task<Single> ReadSingle(this NetworkStream stream) { return BitConverter.ToSingle(Swivel(await stream.ReadBytes(sizeof(Single))), 0); }
        public static async Task<Double> ReadDouble(this NetworkStream stream) { return BitConverter.ToDouble(Swivel(await stream.ReadBytes(sizeof(Double))), 0); }
        public static async Task<string> ReadString(this NetworkStream stream)
        {
            var length = await stream.ReadInt32();
            var data = await stream.ReadBytes(length);
            return Encoding.UTF8.GetString(data);
        }
        public static async Task<T> ReadProtoBuf<T>(this NetworkStream stream)
        {
            var length = await stream.ReadInt32();
            var data = await stream.ReadBytes(length);
            var result = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(data, writable: false));
            return result;
        }
        #endregion
        #region Writing
        public static void Write(this NetworkStream stream, byte value) { stream.WriteByte(value); }
        public static void Write(this NetworkStream stream, byte[] data) { stream.Write(data, 0, data.Length); }
        public static void Write(this NetworkStream stream, Int16 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, Int32 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, Int64 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, UInt16 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, UInt32 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, UInt64 value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, Single value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, Double value) { stream.Write(Swivel(BitConverter.GetBytes(value))); }
        public static void Write(this NetworkStream stream, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            stream.Write(data.Length);
            stream.Write(data);
        }
        public static void WriteProtoBuf<T>(this NetworkStream stream, T input)
        {
            using (var memstream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memstream, input);
                stream.Write((Int32)memstream.Length);
                memstream.Seek(0, SeekOrigin.Begin);
                memstream.CopyTo(stream);
            }
        }
        #endregion
    }
}