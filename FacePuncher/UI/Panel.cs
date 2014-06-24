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

namespace FacePuncher.UI
{
    class Panel : Widget, IWidgetContainer
    {
        private Dictionary<string, Widget> _children { get; set; }

        public Widget this[String name]
        {
            get { return _children[name]; }
        }

        public Panel(String name)
            : this(name, Position.Zero, 0, 0) { }

        public Panel(string name, Position pos, int width, int height,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, width, height, false, fc, bc)
        {
            _children = new Dictionary<string, Widget>();
        }

        public override void Draw()
        {
            DrawChildren();
        }

        public void DrawChildren()
        {
            foreach (var w in _children) {
                w.Value.Draw();
            }
        }

        public override List<Widget> GetSelectableWidgets()
        {
            List<Widget> list = new List<Widget>();
            if (IsSelectable) list.Add(this);

            foreach (var widget in _children) {
                list.AddRange(widget.Value.GetSelectableWidgets());
            }

            return list;
        }

        public void AddChild(Widget w)
        {
            w.Parent = this;
            _children.Add(w.Name, w);
        }

        public void RemoveChild(Widget w)
        {
            _children.Remove(w.Name);
        }

        public bool ContainsChild(String name)
        {
            return _children.ContainsKey(name);
        }
    }
}
