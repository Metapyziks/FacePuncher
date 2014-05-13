using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool Intersects(Position pos)
        {
            return this.Left <= pos.X && this.Top <= pos.Y
                && this.Right > pos.X && this.Bottom > pos.Y;
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

        public Position NearestPosition(Position pos)
        {
            return new Position(pos.X.Clamp(Left, Right - 1), pos.Y.Clamp(Top, Bottom - 1));
        }

        public IEnumerable<Position> Positions
        {
            get
            {
                var rect = this;

                return
                    Enumerable.Range(Top, Height)
                    .SelectMany(y =>
                        Enumerable.Range(rect.Left, rect.Width)
                        .Select(x => new Position(x, y)));
            }
        }

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3})", Left, Top, Width, Height);
        }
    }
}
