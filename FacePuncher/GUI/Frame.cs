using System;
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    /// <summary>
    /// Frame widget.
    /// </summary>
    class Frame : Widget, IWidgetContainer
    {
        /// <summary>
        /// Frame title.
        /// </summary>
        public string Title { get; set; }

        public Dictionary<string, Widget> Children
        { get; set; }

        static readonly char[] _frameChars = new[] {
            (char)205, (char)186, // horizontal, vertical
            (char)201, (char)187, // up-left, up-right
            (char)200, (char)188  // down-left, down-right
        };

        /// <summary>
        /// Creates empty frame.
        /// </summary>
        /// <param name="name">Name of the widget.</param>
        /// <param name="pos">Position of the frame.</param>
        /// <param name="width">Size of the frame.</param>
        /// <param name="height">Size of the frame.</param>
        /// <param name="title">Frame title.</param>
        /// <param name="fc">Foreground color (default gray).</param>
        /// <param name="bc">Background color (default black).</param>
        public Frame(string name, Position pos, int width, int height, string title = "",
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
            : base(name, pos, width, height, false, fc, bc)
        {
            Children = new Dictionary<string, Widget>();
            Title = title;
        }

        public override void Draw()
        {
            DrawChildren();

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

            // Render title
            GuiManager.DrawString(new Position(Position.X + 2, Position.Y), Title);
        }

        public override List<Widget> GetSelectableWidgets()
        {
            List<Widget> list = new List<Widget>();
            if (IsSelectable) list.Add(this);

            foreach (var widget in Children)
            {
                list.AddRange(widget.Value.GetSelectableWidgets());
            }

            return list;
        }

        public void AddChild(Widget w)
        {
            Children.Add(w.Name, w);
        }

        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }
    }
}
