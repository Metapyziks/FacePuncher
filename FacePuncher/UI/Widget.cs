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
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Base class for every widget.
    /// </summary>
    abstract class Widget
    {
        /// <summary>
        /// Name of the widget.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Describes whether widget can be selected.
        /// </summary>
        public bool IsSelectable
        {
            get { return _isSelectable; }
        }
        private bool _isSelectable;

        /// <summary>
        /// Describes whether widget is selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Widgets foreground color.
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Widgets background color.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; }

        protected Rectangle rectangle;

        /// <summary>
        /// Creates an widget.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the widget.</param>
        /// <param name="width">Size of the widget.</param>
        /// <param name="height">Size of the widget.</param>
        /// <param name="isSelectable">If true widget can be selected.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public Widget(string name, Position pos, int width, int height, bool isSelectable,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            this.Name = name;

            this.rectangle = new Rectangle(pos, new Position(pos.X + width, pos.Y + height));

            this.ForegroundColor = fc;
            this.BackgroundColor = bc;

            _isSelectable = isSelectable;
        }

        /// <summary>
        /// Widget's position.
        /// </summary>
        public Position Position
        {
            get { return rectangle.TopLeft; }
        }

        /// <summary>
        /// Widget's width.
        /// </summary>
        public int Width
        {
            get { return rectangle.Width; }
        }

        /// <summary>
        /// Widget's height.
        /// </summary>
        public int Height
        {
            get { return rectangle.Height; }
        }

        /// <summary>
        /// List of widgets that can be selected.
        /// </summary>
        /// <returns>List of selectable widgets.</returns>
        public virtual List<Widget> GetSelectableWidgets()
        {
            List<Widget> list = new List<Widget>();
            if (IsSelectable) list.Add(this);

            return list;
        }

        /// <summary>
        /// Function used to render widget.
        /// </summary>
        abstract public void Draw();
    }
}
