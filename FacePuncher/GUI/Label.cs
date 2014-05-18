using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class Label : Widget
    {
        public string Text { get; set; }

        public Label(string name, Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, 0, 0, false, fc, bc)
        {
            this.Text = text;
        }

        public override void Draw()
        {
            GuiManager.DrawString(Position, Text, ForegroundColor, BackgroundColor);
        }
    }
}
