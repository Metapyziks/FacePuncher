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
using System.Net.Sockets;
using System.Threading.Tasks;

using FacePuncher.Network;

namespace FacePuncher.Graphics
{
    public class EntityAppearance : IEnumerable<EntityAppearance.Frame>
    {
        public struct Frame
        {
            public char Symbol;
            public ConsoleColor ForeColor;
            public ConsoleColor BackColor;
            public int Duration;

            public Frame(char symbol, ConsoleColor foreColor, ConsoleColor backColor, int duration)
            {
                if (duration >= 256)
                {
                    throw new ArgumentException("Frame duration must be less than 256.");
                }
                else if (duration <= 0)
                {
                    throw new ArgumentException("Frame duration must be larger than zero.");
                }

                Symbol = symbol;
                ForeColor = foreColor;
                BackColor = backColor;
                Duration = duration;
            }

            public static bool operator==(Frame a, Frame b)
            {
                return a.Equals(b);
            }

            public static bool operator!=(Frame a, Frame b)
            {
                return !a.Equals(b);
            }

            public static async Task<Frame> Read(NetworkStream stream)
            {
                var appearance = await stream.ReadAppearance();
                return new Frame()
                {
                    Symbol = appearance.Item1,
                    ForeColor = appearance.Item2,
                    BackColor = appearance.Item3,
                    Duration = await stream.ReadByteAsync()
                };
            }

            public override bool Equals(object obj)
            {
                return obj is Frame && Equals((Frame) obj);
            }

            public bool Equals(Frame that)
            {
                return this.Symbol == that.Symbol
                    && this.ForeColor == that.ForeColor
                    && this.BackColor == that.BackColor
                    && this.Duration == that.Duration;
            }

            public override int GetHashCode()
            {
                return Symbol << 16 ^ (int) ForeColor << 12 ^ (int) BackColor << 8 ^ Duration;
            }

            public void WriteToStream(Stream stream)
            {
                stream.WriteAppearance(Symbol, ForeColor, BackColor);
                stream.WriteByte((byte)Duration);
            }
        }

        private List<Frame> _frames;

        public Frame this[int frame]
        {
            get { return _frames[frame]; }
            set { _frames[frame] = value; }
        }

        public int FrameCount
        {
            get { return _frames.Count; }
        }

        public EntityAppearance()
        {
            _frames = new List<Frame>();
        }

        public static async Task<EntityAppearance> Read(NetworkStream stream)
        {
            int count = await stream.ReadByteAsync();
            var result = new EntityAppearance();
            for (int i = 0; i < count; ++i)
            {
                result._frames.Add(await Frame.Read(stream));
            }
            return result;
        }

        public void Add(Frame frame)
        {
            if (_frames.Count >= 255)
            {
                throw new Exception("EntityAppearance cannot have more than 255 frames.");
            }

            _frames.Add(frame);
        }

        public void Add(char symbol, ConsoleColor foreColor, ConsoleColor backColor, int duration)
        {
            Add(new Frame(symbol, foreColor, backColor, duration));
        }

        public void Add(char symbol, int duration)
        {
            var foreColor = FrameCount == 0 ? ConsoleColor.Gray : _frames.Last().ForeColor;
            var backColor = FrameCount == 0 ? ConsoleColor.Black : _frames.Last().BackColor;

            Add(new Frame(symbol, foreColor, backColor, duration));
        }

        public void Add(char symbol)
        {
            Add(symbol, 1);
        }

        public void WriteToStream(Stream stream)
        {
            stream.WriteByte((byte)FrameCount);

            foreach (var frame in this)
            {
                frame.WriteToStream(stream);
            }
        }

        public IEnumerator<EntityAppearance.Frame> GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is EntityAppearance && Equals((EntityAppearance) obj);
        }

        public bool Equals(EntityAppearance that)
        {
            return this.FrameCount == that.FrameCount
                && this._frames.Zip(that._frames, (a, b) => a.Equals(b)).All(x => x);
        }

        public override int GetHashCode()
        {
            return _frames.Aggregate(0, (s, x) => s ^ x.GetHashCode());
        }
    }
}
