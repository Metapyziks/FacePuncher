using FacePuncher.Geometry;
using System;
using System.Collections.Generic;

namespace FacePuncher.GUI
{
    abstract class Widget
    {
        public string Name { get; set; }

        protected Rectangle rectangle;

        private bool _isSelectable;
        public bool IsSelectable
        {
            get { return _isSelectable; }
        }

        public bool IsSelected { get; set; }

        public delegate void WidgetAction();
        public WidgetAction Use = new WidgetAction(() => { });

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public Widget(string name, Position pos, int width, int height, bool isSelectable,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            this.Name = name;

            this.rectangle = new Rectangle(pos, new Position(pos.X + width, pos.Y + height));

            this.ForegroundColor = fc;
            this.BackgroundColor = bc;

            _isSelectable = isSelectable;
        }

        public Position Position
        {
            get { return rectangle.TopLeft; }
        }

        public int Width
        {
            get { return rectangle.Width; }
        }

        public int Height
        {
            get { return rectangle.Height; }
        }

        public virtual List<Widget> GetSelectableWidgets()
        {
            List<Widget> list = new List<Widget>();
            if (IsSelectable) list.Add(this);

            return list;
        }

        abstract public void Draw();
    }
}
