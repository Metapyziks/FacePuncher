using System;

namespace FacePuncher.Geometry
{
    public struct Rectangle
    {
        public int Left;
        public int Top;

        public int Width;
        public int Height;

        public int Right
        {
            get { return Left + Width; }
            set { Width = value - Left; }
        }

        public int Bottom
        {
            get { return Top + Height; }
            set { Height = value - Top; }
        }

        public Position TopLeft
        {
            get { return new Position(Left, Top); }
            set { Left = value.X; Top = value.Y; }
        }
        public Position TopRight
        {
            get { return new Position(Right, Top); }
            set { Right = value.X; Top = value.Y; }
        }
        public Position BottomLeft
        {
            get { return new Position(Left, Bottom); }
            set { Left = value.X; Bottom = value.Y; }
        }
        public Position BottomRight
        {
            get { return new Position(Right, Bottom); }
            set { Right = value.X; Bottom = value.Y; }
        }

        public Rectangle(Position tl, Position br)
        {
            Left = tl.X; Top = tl.Y;

            Width = br.X - tl.X;
            Height = br.Y - tl.Y;
        }

        public Rectangle(int x, int y, int w, int h)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = h;
        }

        public static Rectangle operator +(Rectangle rect, Position offset)
        {
            return new Rectangle(rect.TopLeft + offset, rect.BottomRight + offset);
        }

        public static Rectangle operator -(Rectangle rect, Position offset)
        {
            return new Rectangle(rect.TopLeft - offset, rect.BottomRight - offset);
        }

        public bool Intersects(Rectangle rect)
        {
            return this.Right > rect.Left && this.Bottom > rect.Top
                && rect.Right > this.Left && rect.Bottom > this.Top;
        }

        public Rectangle Intersection(Rectangle rect)
        {
            if (!Intersects(rect)) return new Rectangle();

            var tl = new Position(
                Math.Max(this.Left, rect.Left),
                Math.Max(this.Top, rect.Top));

            var br = new Position(
                Math.Min(this.Right, rect.Right),
                Math.Min(this.Bottom, rect.Bottom));

            var diff = br - tl;

            return new Rectangle(tl.X, tl.Y, diff.X, diff.Y);
        }
    }
}
