using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class Label : Widget
    {
        public string Text { get; set; }

        public Label(Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(pos, 0, 0, false, fc, bc)
        {
            this.Text = text;
        }

        public override void Draw()
        {
            GuiManager.DrawString(Position, Text, ForegroundColor, BackgroundColor);
        }
    }
}
