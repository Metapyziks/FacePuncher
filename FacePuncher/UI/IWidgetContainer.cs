using System;
using System.Collections.Generic;

namespace FacePuncher.UI
{
    /// <summary>
    /// Interface for widgets, that can store other widgets.
    /// </summary>
    interface IWidgetContainer
    {
        /// <summary>
        /// Stored widgets.
        /// 
        /// key - name of the widget.
        /// value - stored widget.
        /// </summary>
        Dictionary<string, Widget> Children { get; set; }

        /// <summary>
        /// Adds widget.
        /// </summary>
        /// <param name="w">Widget to store.</param>
        void AddChild(Widget w);

        /// <summary>
        /// Renders stored widgets.
        /// </summary>
        void DrawChildren();
    }
}
