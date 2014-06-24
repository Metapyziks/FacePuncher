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

namespace FacePuncher.UI
{
    /// <summary>
    /// Interface for widgets, that can store other widgets.
    /// </summary>
    interface IWidgetContainer
    {
        Widget this[String name] { get; }

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
}
