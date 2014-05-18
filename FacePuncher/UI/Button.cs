using System;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Widget that performs an action when used.
    /// </summary>
    class Button : UsableWidget
    {
        /// <summary>
        /// The text shown on the button.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates new button.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the button.</param>
        /// <param name="text">The text shown on the button.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public Button(string name, Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, text.Length, 1, fc, bc)
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

            UIManager.DrawString(Position, Text, fc, bc);
        }
    }
}
