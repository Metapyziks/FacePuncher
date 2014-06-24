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
using System.Collections.Generic;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher.UI
{
    /// <summary>
    /// Frame widget.
    /// </summary>
    class Frame : Panel
    {
        /// <summary>
        /// Frame title.
        /// </summary>
        public string Title { get; set; }

        static readonly char[] _frameChars = new[] {
            (char)205, (char)186, // horizontal, vertical
            (char)201, (char)187, // up-left, up-right
            (char)200, (char)188  // down-left, down-right
        };

        public Frame(String name)
            : this(name, Position.Zero, 0, 0) { }

        /// <summary>
        /// Creates empty frame.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the frame.</param>
        /// <param name="width">Size of the frame.</param>
        /// <param name="height">Size of the frame.</param>
        /// <param name="title">Frame title.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public Frame(string name, Position pos, int width, int height, string title = "",
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, width, height, fc, bc)
        {
            Title = title;
        }

        public override void Draw()
        {
            base.Draw();

            // Render frame corners
            Interface.Display.SetCell(ScreenRectangle.TopLeft, _frameChars[2], ForeColor, BackColor);
            Interface.Display.SetCell(ScreenRectangle.TopRight - Position.UnitX, _frameChars[3], ForeColor, BackColor);
            Interface.Display.SetCell(ScreenRectangle.BottomLeft - Position.UnitY, _frameChars[4], ForeColor, BackColor);
            Interface.Display.SetCell(ScreenRectangle.BottomRight - Position.UnitX - Position.UnitY, _frameChars[5], ForeColor, BackColor);

            // Render horizontal line
            for (var x = 1; x < rectangle.Width - 1; x++)
            {
                Interface.Display.SetCell(ScreenRectangle.Left + x, ScreenRectangle.Top, _frameChars[0], ForeColor, BackColor);
                Interface.Display.SetCell(ScreenRectangle.Left + x, ScreenRectangle.Bottom - 1, _frameChars[0], ForeColor, BackColor);
            }

            // Render vertical line
            for (var y = 1; y < rectangle.Height - 1; y++)
            {
                Interface.Display.SetCell(ScreenRectangle.Left, ScreenRectangle.Top + y, _frameChars[1], ForeColor, BackColor);
                Interface.Display.SetCell(ScreenRectangle.Right - 1, ScreenRectangle.Top + y, _frameChars[1], ForeColor, BackColor);
            }

            // Render title
            UIManager.DrawString(new Position(ScreenPosition.X + 2, ScreenPosition.Y), Title);
        }
    }
}
