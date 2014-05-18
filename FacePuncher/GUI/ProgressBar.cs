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
        public float Value { get; set; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Creates progress bar.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the progress bar.</param>
        /// <param name="width">Width of the progress bar</param>
        /// <param name="maxValue">Maximum value.</param>
        /// <param name="value">Current value (default: 0).</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public ProgressBar(string name, Position pos, int width, int maxValue, float value = 0.0f,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, width, 1, false, fc, bc)
        {
            this.Value = value;
            this.MaxValue = maxValue;
        }

        public override void Draw()
        {
            // Draws background
            for (var x = 0; x < Width; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, BackgroundColor, ConsoleColor.Black);
            }

            int progress = (int)((float)(Value / MaxValue) * Width);

            // Draws actual progress
            for (var x = 0; x < progress; x++)
            {
                Display.SetCell(Position.X + x, Position.Y, (char)219, ForegroundColor, ConsoleColor.Black);
            }
        }
    }
}
