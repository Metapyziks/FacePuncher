/* Copyright (c) 2014 Michał Ferchow [deseteral@gmail.com]
 * Copyright (c) 2014 Saša Barišić [cartman300@net.hr]
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
    /// Widget used for getting user text input.
    /// </summary>
    class TextBox : UsableWidget
    {
        /// <summary>
        /// Current value.
        /// </summary>
        [ScriptDefinable]
        public string Text { get; set; }

        [ScriptDefinable]
        public int Length
        {
            get { return Width; }
            set { Width = value; }
        }

        private bool _isEdited;

        public TextBox(String name)
            : this(name, Position.Zero, 8, "Text Box") { }

        /// <summary>
        /// Creates empty text box.
        /// </summary>
        /// <param name="name">Name of the widget</param>
        /// <param name="pos">Position of the text box</param>
        /// <param name="length">Maximum width</param>
        /// <param name="text">Value of textbox (default: empty).</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public TextBox(string name, Position pos, int length, string text = "",
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, length, 1, fc, bc)
        {
            this.Text = text;
            this._isEdited = false;
        }

        protected override void OnUse()
        {
            _isEdited = true;
            InterceptInput = true;
            Text = "";
        }

        protected override void OnDraw()
        {
            ConsoleColor fc = ForeColor;
            ConsoleColor bc = BackColor;

            // If widget is selected
            // swap colors
            if (IsSelectable && IsSelected && !_isEdited)
            {
                fc = BackColor;
                bc = ForeColor;
            }

            ConsoleKeyInfo info;
            if (_isEdited && Interface.Input.TryReadKey(out info)) {
                // ASCII code of pressed key
                int asciiCode = (int)info.KeyChar;

                // If pressed key is enter - stop editing
                if (info.Key == ConsoleKey.Enter) {
                    _isEdited = false;
                    InterceptInput = false;
                }
                    // If pressed key is backspace - remove last char
                else if (info.Key == ConsoleKey.Backspace) {
                    if (Text.Length > 0)
                        Text = Text.Remove(Text.Length - 1);
                }
                    // If pressed key is a printable char
                    // add this char to text
                else if (asciiCode >= 32 && asciiCode <= 126) {
                    Text += info.KeyChar.ToString();
                }
            }

            UIManager.DrawString(ScreenPosition, "[" + Text + "]", fc, bc);

            if (_isEdited) {
                var CursorPos = ScreenPosition;
                CursorPos.X += Text.Length + 1;

                var cfc = fc;
                if (((long)Tools.CurTime()) % 2 == 0)
                    cfc = bc;

                UIManager.DrawString(CursorPos, "_", cfc, bc);
            }
        }
    }
}
