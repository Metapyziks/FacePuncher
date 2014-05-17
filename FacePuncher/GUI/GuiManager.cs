using System;
using System.Collections.Generic;
using FacePuncher.Geometry;

namespace FacePuncher.GUI
{
    class GuiManager : IWidgetContainer
    {
        public GuiManager()
        {
            Children = new Dictionary<string, Widget>();
        }

        public Dictionary<string, Widget> Children
        { get; set; }

        public void AddChild(string name, Widget w)
        {
            Children.Add(name, w);
        }

        public void Draw()
        {
            DrawChildren();
        }

        public void DrawChildren()
        {
            foreach (var w in Children)
            {
                w.Value.Draw();
            }
        }

        public static void DrawString(Position pos, string text,
            ConsoleColor fc = ConsoleColor.Gray, ConsoleColor bc = ConsoleColor.Black)
        {
            for (var x = 0; x < text.Length; x++)
            {
                Display.SetCell(pos.X + x, pos.Y, text[x], fc, bc);
            }
        }
    }
}
