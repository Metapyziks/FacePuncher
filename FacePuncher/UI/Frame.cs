/* Copyright (c) 2014 Michał Ferchow [deseteral@gmail.com]
 * 
 * This file is part of FacePuncher.
 * 
 * FacePuncher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * FacePuncher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
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
    class Frame : Widget, IWidgetContainer
    {
        /// <summary>
        /// Frame title.
        /// </summary>
        public string Title { get; set; }

        public Dictionary<string, Widget> Children
        { get; set; }

        static readonly char[] _frameChars = new[] {
            (char)205, (char)186, // horizontal, vertical
            (char)201, (char)187, // up-left, up-right
            (char)200, (char)188  // down-left, down-right
        };

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
            : base(name, pos, width, height, false, fc, bc)
        {
            Children = new Dictionary<string, Widget>();
            Title = title;
        }

        public override void Draw()
        {
            DrawChildren();

            var parentX = Parent.Position.X;
            var parentY = Parent.Position.Y;

            // Render frame corners
            Interface.Display.SetCell(Parent.Position + rectangle.TopLeft, _frameChars[2], ForegroundColor, BackgroundColor);
            Interface.Display.SetCell(Parent.Position + rectangle.TopRight, _frameChars[3], ForegroundColor, BackgroundColor);
            Interface.Display.SetCell(Parent.Position + rectangle.BottomLeft, _frameChars[4], ForegroundColor, BackgroundColor);
            Interface.Display.SetCell(Parent.Position + rectangle.BottomRight, _frameChars[5], ForegroundColor, BackgroundColor);

            // Render horizontal line
            for (var x = 0; x < rectangle.Width - 1; x++)
            {
                Interface.Display.SetCell(parentX + x + rectangle.Left + 1, parentY + rectangle.Top, _frameChars[0], ForegroundColor, BackgroundColor);
                Interface.Display.SetCell(parentX + x + rectangle.Left + 1, parentY + rectangle.Bottom, _frameChars[0], ForegroundColor, BackgroundColor);
            }

            // Render vertical line
            for (var y = 0; y < rectangle.Height - 1; y++)
            {
                Interface.Display.SetCell(parentX + rectangle.Left, parentY + y + rectangle.Top + 1, _frameChars[1], ForegroundColor, BackgroundColor);
                Interface.Display.SetCell(parentX + rectangle.Right, parentY + y + rectangle.Top + 1, _frameChars[1], ForegroundColor, BackgroundColor);
            }

            // Render title
            UIManager.DrawString(new Position(parentX + Position.X + 2, parentY + Position.Y), Title);
        }

        public override List<Widget> GetSelectableWidgets()
        {
            List<Widget> list = new List<Widget>();
            if (IsSelectable) list.Add(this);

            foreach (var widget in Children)
            {
                list.AddRange(widget.Value.GetSelectableWidgets());
            }

            return list;
        }

        public void AddChild(Widget w)
        {
            w.Parent = this;
            Children.Add(w.Name, w);
        }

        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }
    }
}
