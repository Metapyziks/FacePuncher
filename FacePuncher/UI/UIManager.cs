/* Copyright (C) 2014 Michał Ferchow (deseteral@gmail.com)
 * Copyright (C) 2014 James King (metapyziks@gmail.com)
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
using FacePuncher.Graphics;

namespace FacePuncher.UI
{
    enum UINavigation
    {
        Up,
        Down,
        Select
    }

    /// <summary>
    /// Root node for all widgets.
    /// </summary>
    class UIManager : IWidgetContainer
    {
        public static readonly Dictionary<ConsoleKey, UINavigation> _sNavigationKeys = new Dictionary<ConsoleKey, UINavigation> {
            { ConsoleKey.W, UINavigation.Up },
            { ConsoleKey.UpArrow, UINavigation.Up },
            { ConsoleKey.NumPad8, UINavigation.Up },
            { ConsoleKey.Subtract, UINavigation.Up },

            { ConsoleKey.S, UINavigation.Down },
            { ConsoleKey.DownArrow, UINavigation.Down },
            { ConsoleKey.NumPad2, UINavigation.Down },
            { ConsoleKey.Add, UINavigation.Down },

            { ConsoleKey.Enter, UINavigation.Select }
        };

        /// <summary>
        /// Is one of widgets blocking input
        /// </summary>
        public static bool IsInputBlocked { get; set; }

        private List<Widget> _selectableWidgets;
        private int _selectedId;

        /// <summary>
        /// Creates empty manager.
        /// </summary>
        public UIManager()
        {
            Children = new Dictionary<string, Widget>();
            _selectableWidgets = new List<Widget>();
            _selectedId = 0;
        }

        public Dictionary<string, Widget> Children
        { get; set; }

        /// <summary>
        /// Function used to render widgets and check for UI input.
        /// </summary>
        public void Draw()
        {
            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;

            // Deselect currently selected item
            if (_selectableWidgets.Count > 0)
                _selectableWidgets[_selectedId].IsSelected = false;

            if (!IsInputBlocked && Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                if (_sNavigationKeys.ContainsKey(key)) {
                    switch (_sNavigationKeys[key]) {
                        case UINavigation.Up:
                            _selectedId--; break;
                        case UINavigation.Down:
                            _selectedId++; break;
                        case UINavigation.Select:
                            if (_selectableWidgets[_selectedId] is UsableWidget) {
                                ((UsableWidget) _selectableWidgets[_selectedId]).Use();
                            }
                            break;
                    }
                }
            }

            if (_selectedId > _selectableWidgets.Count - 1)
                _selectedId = _selectableWidgets.Count - 1;
            else if (_selectedId < 0)
                _selectedId = 0;

            if (_selectableWidgets.Count > 0)
                _selectableWidgets[_selectedId].IsSelected = true;

            DrawChildren();
        }

        /// <summary>
        /// Used to render widgets stored in manager.
        /// </summary>
        public void DrawChildren()
        {
            foreach (var w in Children)
            {
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
            for (var x = 0; x < text.Length; x++)
            {
                Display.SetCell(pos.X + x, pos.Y, text[x], fc, bc);
            }
        }

        /// <summary>
        /// Calculates information about widgets that can be selected.
        /// Call only once, after adding or removing widgets.
        /// </summary>
        /// <returns>Number of widgets that can be selected.</returns>
        public int CalculateSelectableWidgets()
        {
            _selectableWidgets = new List<Widget>();

            foreach (var widget in Children)
            {
                _selectableWidgets.AddRange(widget.Value.GetSelectableWidgets());
            }

            _selectedId = 0;

            if (_selectableWidgets.Count > 0)
                _selectableWidgets[_selectedId].IsSelected = true;

            return _selectableWidgets.Count;
        }

        public void AddChild(Widget w)
        {
            Children.Add(w.Name, w);
        }

        public void RemoveChild(Widget w)
        {
            Children.Remove(w.Name);
        }
    }
}
