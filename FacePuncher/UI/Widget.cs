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
using System.Reflection;
using System.Xml.Linq;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    delegate void WidgetConstructorDelegate(Widget widget);

    /// <summary>
    /// Base class for every widget.
    /// </summary>
    abstract class Widget : IDefinitionLoadable
    {
        private sealed class ClassInfo
        {
            public String Name { get; private set; }

            public String Base { get; private set; }

            public WidgetConstructorDelegate Constructor { get; private set; }
            
            public ClassInfo(String name, String baseName, WidgetConstructorDelegate ctor)
            {
                Name = name;
                Base = baseName;
                Constructor = ctor;
            }
        }

        private static Dictionary<String, ClassInfo> _sWidgetCtors
            = new Dictionary<string, ClassInfo>();

        private static Dictionary<String, ConstructorInfo> _sBaseCtors
            = new Dictionary<string, ConstructorInfo>();

        static Widget()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (!typeof(Widget).IsAssignableFrom(type)) continue;
                if (type.IsAbstract) continue;

                var ctor = type.GetConstructor(new Type[] { typeof(String) });
                if (ctor == null) continue;

                _sBaseCtors.Add(type.Name, ctor);
            }

            Definitions.RegisterType("widget", elem => {
                WidgetConstructorDelegate ctor = widget => {
                    widget.LoadFromDefinition(elem);
                };
                
                var typeName = elem.Attribute("name").Value;
                var baseName = elem.HasAttribute("base") ? elem.Attribute("base").Value : "Frame";

                Register(typeName, baseName, ctor);
            });
        }

        public static void Register(String typeName, String baseName, WidgetConstructorDelegate ctor)
        {
            _sWidgetCtors.Add(typeName, new ClassInfo(typeName, baseName, ctor));
        }

        public static String GetBaseName(String className)
        {
            return _sWidgetCtors[className].Base;
        }

        public static bool Extends(String className, String baseName)
        {
            if (className == baseName) return true;
            if (!_sWidgetCtors.ContainsKey(className)) return false;

            return Extends(_sWidgetCtors[className].Base, baseName);
        }

        public static Widget Create(String typeName, String name)
        {
            Widget widget;

            if (_sWidgetCtors.ContainsKey(typeName)) {
                var info = _sWidgetCtors[typeName];
                widget = Create(info.Base, name);
                info.Constructor(widget);
            } else {
                widget = (Widget) _sBaseCtors[typeName].Invoke(new Object[] { name });
            }

            return widget;
        }

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
        [ScriptDefinable]
        public ConsoleColor ForeColor { get; set; }

        /// <summary>
        /// Widgets background color.
        /// </summary>
        [ScriptDefinable]
        public ConsoleColor BackColor { get; set; }

        /// <summary>
        /// Widgets parent.
        /// </summary>
        public Widget Parent { get; set; }

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

            this.ForeColor = fc;
            this.BackColor = bc;

            _isSelectable = isSelectable;
        }

        /// <summary>
        /// Widget's position.
        /// </summary>
        public Position Position
        {
            get { return rectangle.TopLeft; }
            set { rectangle.TopLeft = value; }
        }

        [ScriptDefinable]
        public int Left
        {
            get { return rectangle.Left; }
            set { rectangle.Left = value; }
        }

        [ScriptDefinable]
        public int Top
        {
            get { return rectangle.Top; }
            set { rectangle.Top = value; }
        }

        /// <summary>
        /// Widget's width.
        /// </summary>
        [ScriptDefinable]
        public int Width
        {
            get { return rectangle.Width; }
            set { rectangle.Width = value; }
        }

        /// <summary>
        /// Widget's height.
        /// </summary>
        [ScriptDefinable]
        public int Height
        {
            get { return rectangle.Height; }
            set { rectangle.Height = value; }
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

        public virtual void LoadFromDefinition(XElement elem)
        {
            Definitions.LoadProperties(this, elem);

            if (elem.HasElement("CenterToParent")) {
                int parentWidth = Parent != null ? Parent.Width : Interface.Display.Width;
                int parentHeight = Parent != null ? Parent.Height : Interface.Display.Height;

                Left = (parentWidth - Width) / 2;
                Top = (parentHeight - Height) / 2;
            }

            if (this is IWidgetContainer && elem.HasElement("Children")) {
                var container = (IWidgetContainer) this;

                foreach (var childElem in elem.Element("Children").Elements()) {
                    var name = childElem.Attribute("name").Value;
                    Widget child;

                    if (container.ContainsChild(name)) {
                        child = container[name];
                    } else {
                        child = Create(childElem.Name.LocalName, name);
                        container.AddChild(child);
                    }

                    child.LoadFromDefinition(childElem);
                }
            }
        }
    }
}
