using FacePuncher.Geometry;
using System;

namespace FacePuncher.UI
{
    /// <summary>
    /// Defines a widget that can be used.
    /// </summary>
    abstract class UsableWidget : Widget
    {
        public delegate void WidgetAction();

        /// <summary>
        /// Function delegate invoked when widget is selected and used.
        /// </summary>
        public WidgetAction Use = new WidgetAction(() => { });

        /// <summary>
        /// Creates an widget that can be used.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the widget.</param>
        /// <param name="width">Size of the widget.</param>
        /// <param name="height">Size of the widget.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public UsableWidget(string name, Position pos, int width, int height,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, width, height, true, fc, bc)
        { }
    }
}
