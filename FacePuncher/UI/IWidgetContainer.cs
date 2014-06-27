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
using System.Linq;

using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Interface for widgets, that can store other widgets.
    /// </summary>
    interface IWidgetContainer
    {
        IEnumerable<Widget> Children { get; }

        Widget this[String name] { get; }

        UIManager Manager { get; }

        Position ScreenPosition { get; }

        int Width { get; }

        int Height { get; }

        /// <summary>
        /// Adds widget.
        /// </summary>
        /// <param name="w">Widget to store.</param>
        void AddChild(Widget w);

        void RemoveChild(Widget w);

        bool ContainsChild(String name);

        /// <summary>
        /// Renders stored widgets.
        /// </summary>
        void DrawChildren();
    }

    static class WidgetContainerExtensions
    {
        public static Widget[] GetSelectableWidgets(this IWidgetContainer container)
        {
            return container.Children
                .SelectMany(x => {
                    if (x.IsHidden) return Enumerable.Empty<Widget>();

                    var widgets = new List<Widget>();
                    if (x.IsSelectable) widgets.Add(x);

                    if (x is IWidgetContainer) {
                        widgets.AddRange(((IWidgetContainer) x).GetSelectableWidgets());
                    }

                    return widgets;
                }).ToArray();
        }

        public static Widget GetNextSelectable(this IWidgetContainer container, Widget widget)
        {
            var selectable = container.GetSelectableWidgets();

            if (selectable.Length == 0) return null;
            if (widget == null) return selectable.First();

            int index = Array.IndexOf(selectable, widget);

            return selectable[(index + 1) % selectable.Length];
        }

        public static Widget GetPrevSelectable(this IWidgetContainer container, Widget widget)
        {
            var selectable = container.GetSelectableWidgets();

            if (selectable.Length == 0) return null;
            if (widget == null) return selectable.First();

            int index = Array.IndexOf(selectable, widget);

            return selectable[(index + selectable.Length - 1) % selectable.Length];
        }
    }
}
