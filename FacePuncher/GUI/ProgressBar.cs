using System;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    /// <summary>
    /// Widget that provides a horizontal progress bar.
    /// </summary>
    class ProgressBar : Widget
    {
        /// <summary>
        /// Current value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Creates progress bar.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the progress bar.</param>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="value">Current value (default: 0).</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public ProgressBar(string name, Position pos, int maxValue, int value = 0,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, maxValue, 1, false, fc, bc)
        {
            this.Value = value;
            this.MaxValue = maxValue;
        }

        public override void Draw()
        {
            // Draws background
            for (var x = 0; x < MaxValue; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, BackgroundColor, ConsoleColor.Black);
            }

            // Draws actual progress
            for (var x = 0; x < Value; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, ForegroundColor, ConsoleColor.Black);
            }
        }
    }
}
