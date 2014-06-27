/* Copyright (c) 2014 Michał Ferchow [deseteral@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with FacePuncher. If not, see <http://www.gnu.org/licenses/>.
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

        protected override void OnDraw()
        {
            // Draws background
            for (var x = 0; x < Width; x++) {
                Interface.Display.SetCell(ScreenPosition.X + x, ScreenPosition.Y, (char) 219, BackColor, ConsoleColor.Black);
            }

            int progress = (int) ((float) (Value / MaxValue) * Width);

            // Draws actual progress
            for (var x = 0; x < progress; x++) {
                Interface.Display.SetCell(ScreenPosition.X + x, ScreenPosition.Y, (char) 219, ForeColor, ConsoleColor.Black);
            }
        }
    }
}
