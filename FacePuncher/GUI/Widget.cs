using FacePuncher.Geometry;
using System;

namespace FacePuncher.GUI
{
    abstract class Widget
    {
        protected Rectangle rectangle;

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public Widget(Position pos, int width, int height, ConsoleColor fc, ConsoleColor bc)
        {
            this.rectangle = new Rectangle(pos, new Position(pos.X + width, pos.Y + height));

            this.ForegroundColor = fc;
            this.BackgroundColor = bc;
        }

        public Widget(Position pos, int width, int height)
            : this(pos, width, height, ConsoleColor.Gray, ConsoleColor.Black)
        {
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
