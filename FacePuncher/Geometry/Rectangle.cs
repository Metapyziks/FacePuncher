namespace FacePuncher.Geometry
{
    struct Rectangle
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

        public Position TopLeft { get { return new Position(Left, Top); } }
        public Position TopRight { get { return new Position(Right, Top); } }
        public Position BottomLeft { get { return new Position(Left, Bottom); } }
        public Position BottomRight { get { return new Position(Right, Bottom); } }

        public Rectangle(int x, int y, int w, int h)
        {
            Left = x;
            Top = y;
            Width = w;
            Height = h;
        }
    }
}
