using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    abstract class Widget
    {
        protected Rectangle rectangle;

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

        public Widget(Position pos, int width, int height)
        {
            this.rectangle = new Rectangle(pos, new Position(pos.X + width, pos.Y + height));
        }

        abstract public void Draw();
    }
}
