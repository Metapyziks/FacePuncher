using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class ProgressBar : Widget
    {
        public int Value { get; set; }
        public int MaxValue { get; set; }

        public ProgressBar(Position pos, int maxValue, int value = 0,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(pos, 0, maxValue, false, fc, bc)
        {
            this.Value = value;
            this.MaxValue = maxValue;
        }

        public override void Draw()
        {
            for (var x = 0; x < MaxValue; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, BackgroundColor, ConsoleColor.Black);
            }

            for (var x = 0; x < Value; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, ForegroundColor, ConsoleColor.Black);
            }
        }
    }
}
