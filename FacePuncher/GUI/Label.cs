using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    /// <summary>
    /// Widget used to display text.
    /// </summary>
    class Label : Widget
    {
        /// <summary>
        /// Text of the label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates new label.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the label.</param>
        /// <param name="text">Text of the label.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public Label(string name, Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, text.Length, 1, false, fc, bc)
        {
            this.Text = text;
        }

        public override void Draw()
        {
            GuiManager.DrawString(Position, Text, ForegroundColor, BackgroundColor);
        }
    }
}
