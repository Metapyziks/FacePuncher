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
using System.Collections.Generic;

using FacePuncher.Geometry;
using FacePuncher.Graphics;

namespace FacePuncher.UI
{
    public enum UINavigation
    {
        Option1 = 1,
        Option2,
        Option3,
        Option4,
        Option5,
        Option6,
        Option7,
        Option8,
        Option9,
        Option10,

        Up = 11,
        Down,
        Select,
        Escape
    }

    /// <summary>
    /// Root node for all widgets.
    /// </summary>
    class UIManager : IWidgetContainer
    {
        public Widget Selected { get; internal set; }

        public bool IsInputIntercepted { get { return Selected != null && Selected.InterceptInput; } }
        
        public IEnumerable<Widget> Children { get { return _children.Values; } }

        public UIManager Manager { get { return this; } }

        public Position ScreenPosition { get { return Position.Zero; } }

        public int Width { get { return Interface.Display.Width; } }

        public int Height { get { return Interface.Display.Height; } }

        private Dictionary<string, Widget> _children { get; set; }

        public Widget this[String name] { get { return _children[name]; } }

        /// <summary>
        /// Creates empty manager.
        /// </summary>
        public UIManager()
        {
            _children = new Dictionary<string, Widget>();
        }

        internal bool Select(Widget widget)
        {
            if (Selected == widget) return false;

            if (Selected != null) {
                Selected.Deselect();
            }

            Selected = widget;

            return true;
        }

        internal bool Deselect(Widget widget)
        {
            if (Selected != widget) return false;

            Selected = null;

            return true;
        }

        /// <summary>
        /// Function used to render widgets and check for UI input.
        /// </summary>
        public void Draw()
        {
            UINavigation nav;
            if (!IsInputIntercepted && Interface.Input.TryReadUINavigation(out nav)) {
                switch (nav) {
                    case UINavigation.Up: {
                        var prev = this.GetPrevSelectable(Selected);
                        if (prev != null) prev.Select();
                    } break;
                    case UINavigation.Down: {
                        var next = this.GetNextSelectable(Selected);
                        if (next != null) next.Select();
                    } break;
                    case UINavigation.Select: {
                        if (Selected is UsableWidget) {
                            ((UsableWidget) Selected).Use();
                        }
                    } break;
                }
            }

            DrawChildren();
        }

        /// <summary>
        /// Used to render widgets stored in manager.
        /// </summary>
        public void DrawChildren()
        {
            foreach (var w in _children) {
                w.Value.Draw();
            }
        }

        /// <summary>
        /// Renders text
        /// </summary>
        /// <param name="pos">Position of first character.</param>
        /// <param name="text">Text to render.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public static void DrawString(Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            for (var x = 0; x < text.Length; x++) {
                Interface.Display.SetCell(pos.X + x, pos.Y, text[x], fc, bc);
            }
        }

        public void AddChild(Widget w)
        {
            _children.Add(w.Name, w);
            w.SetParent(this);
        }

        public void RemoveChild(Widget w)
        {
            _children.Remove(w.Name);
            w.SetParent(null);
        }

        public bool ContainsChild(String name)
        {
            return _children.ContainsKey(name);
        }
    }
}
