using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                if (duration >= 256) {
                    throw new ArgumentException("Frame duration must be less than 256.");
                } else if (duration <= 0) {
                    throw new ArgumentException("Frame duration must be larger than zero.");
                }

                Symbol = symbol;
                ForeColor = foreColor;
                BackColor = backColor;
                Duration = duration;
            }

            public Frame(Stream stream)
            {
                stream.ReadAppearance(out Symbol, out ForeColor, out BackColor);

                Duration = stream.ReadByte();
            }

            public void WriteToStream(Stream stream)
            {
                stream.WriteAppearance(Symbol, ForeColor, BackColor);
                stream.WriteByte((byte) Duration);
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

        public EntityAppearance(Stream stream)
        {
            int count = stream.ReadByte();

            for (int i = 0; i < count; ++i) {
                _frames.Add(new Frame(stream));
            }
        }

        public void Add(Frame frame)
        {
            if (_frames.Count >= 255) {
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
            stream.WriteByte((byte) FrameCount);

            foreach (var frame in this) {
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
    }
}
