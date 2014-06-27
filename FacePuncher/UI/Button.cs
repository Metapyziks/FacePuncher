/* Copyright (c) 2014 Michał Ferchow [deseteral@gmail.com]
 * Copyright (c) 2014 James King [metapyziks@gmail.com]
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
    /// Widget that performs an action when used.
    /// </summary>
    class Button : UsableWidget
    {
        private String _text;

        /// <summary>
        /// The text shown on the button.
        /// </summary>
        [ScriptDefinable]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                Width = value.Length;
            }
        }

        public Button(String name)
            : this(name, Position.Zero, "Button") { }

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

        protected override void OnDraw()
        {
            ConsoleColor fc = ForeColor;
            ConsoleColor bc = BackColor;

            // If widget is selected
            // swap colors
            if (IsSelectable && IsSelected)
            {
                fc = BackColor;
                bc = ForeColor;
            }

            UIManager.DrawString(ScreenPosition, Text, fc, bc);
        }
    }
}
