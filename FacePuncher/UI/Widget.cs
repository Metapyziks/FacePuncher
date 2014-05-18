using System;
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.UI
{
    /// <summary>
    /// Base class for every widget.
    /// </summary>
    abstract class Widget
    {
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

        public delegate void WidgetAction();
        /// <summary>
        /// Function delegate invoked when widget is selected and used.
        /// </summary>
        public WidgetAction Use = new WidgetAction(() => { });

        /// <summary>
        /// Widgets foreground color.
        /// </summary>
        public ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Widgets background color.
        /// </summary>
        public ConsoleColor BackgroundColor { get; set; }

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

            this.ForegroundColor = fc;
            this.BackgroundColor = bc;

            _isSelectable = isSelectable;
        }

        /// <summary>
        /// Widget's position.
        /// </summary>
        public Position Position
        {
            get { return rectangle.TopLeft; }
        }

        /// <summary>
        /// Widget's width.
        /// </summary>
        public int Width
        {
            get { return rectangle.Width; }
        }

        /// <summary>
        /// Widget's height.
        /// </summary>
        public int Height
        {
            get { return rectangle.Height; }
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
    }
}
