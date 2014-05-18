/* Copyright (C) 2014 Michał Ferchow (deseteral@gmail.com)
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher.UI
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
