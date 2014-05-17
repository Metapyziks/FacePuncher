using FacePuncher.Geometry;
using System;

namespace FacePuncher.GUI
{
    abstract class Widget
    {
        protected Rectangle rectangle;

        private bool _isSelectable;
        public bool IsSelectable
        {
            get { return _isSelectable; }
        }

        public bool IsSelected { get; set; }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public Widget(Position pos, int width, int height, bool isSelectable,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
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

        abstract public void Draw();
    }
}
