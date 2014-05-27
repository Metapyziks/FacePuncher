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

namespace FacePuncher.UI
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
            UIManager.DrawString(Parent.Position + Position, Text, ForegroundColor, BackgroundColor);
        }
    }
}
