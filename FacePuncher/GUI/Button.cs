using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class Button : Widget
    {
        public string Text { get; set; }

        public Button(string name, Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, 0, 0, true, fc, bc)
        {
            this.Text = text;
        }

        public override void Draw()
        {
            ConsoleColor fc = ForegroundColor;
            ConsoleColor bc = BackgroundColor;

            // If widget is selected
            // swap colors
            if (IsSelectable && IsSelected)
            {
                fc = BackgroundColor;
                bc = ForegroundColor;
            }

            GuiManager.DrawString(Position, Text, fc, bc);
        }
    }
}
