using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class Label : Widget
    {
        public string Text { get; set; }

        public Label(Position pos, string text)
            : base(pos, 0, 0)
        {
            this.Text = text;
        }

        public Label(Position pos, string text, ConsoleColor fc, ConsoleColor bc)
            : base(pos, 0, 0, fc, bc)
        {
            this.Text = text;
        }

        public override void Draw()
        {
            for (var x = 0; x < Text.Length; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, Text[x], ForegroundColor, BackgroundColor);
            }
        }
    }
}
