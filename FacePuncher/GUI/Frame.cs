using FacePuncher.Geometry;
using System;

namespace FacePuncher.GUI
{
    class Frame : Widget
    {
        static readonly char[] _frameChars = new[] {
            (char)205, (char)186, // horizontal, vertical
            (char)201, (char)187, // up-left, up-right
            (char)200, (char)188  // down-left, down-right
        };

        public Frame(Position pos, int width, int height,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(pos, width, height, fc, bc)
        { }

        public override void Draw()
        {
            // Render frame corners
            Display.SetCell(rectangle.TopLeft, _frameChars[2], ForegroundColor, BackgroundColor);
            Display.SetCell(rectangle.TopRight, _frameChars[3], ForegroundColor, BackgroundColor);
            Display.SetCell(rectangle.BottomLeft, _frameChars[4], ForegroundColor, BackgroundColor);
            Display.SetCell(rectangle.BottomRight, _frameChars[5], ForegroundColor, BackgroundColor);

            // Render horizontal line
            for (var x = 0; x < rectangle.Width - 1; x++)
            {
                Display.SetCell(x + rectangle.Left + 1, rectangle.Top, _frameChars[0], ForegroundColor, BackgroundColor);
                Display.SetCell(x + rectangle.Left + 1, rectangle.Bottom, _frameChars[0], ForegroundColor, BackgroundColor);
            }

            // Render vertical line
            for (var y = 0; y < rectangle.Height - 1; y++)
            {
                Display.SetCell(rectangle.Left, y + rectangle.Top + 1, _frameChars[1], ForegroundColor, BackgroundColor);
                Display.SetCell(rectangle.Right, y + rectangle.Top + 1, _frameChars[1], ForegroundColor, BackgroundColor);
            }
        }
    }
}
